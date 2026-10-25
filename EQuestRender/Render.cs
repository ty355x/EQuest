using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender.Shaders;

namespace EQuestRender
{
	public class Render : IDisposable
	{
		public GraphicsDeviceManager GDM { get => _gdm; }
		public GraphicsDevice Graphics { get => _gdm.GraphicsDevice; }
		public int Width { get; private set; }
		public int Height { get; private set; }

		public BaseShader Shader { get => _shader; set => ChangeShader(value); }
		public Camera Camera { get; private set; }
		public Texture2D DefaultTexture { get; set; }
		public TransparencyRenderMode TransparencyRenderMode { get; set; }
		public bool LastMeshHasTransparentTextures { get; private set; }
		public bool UpdateTransformForMesh { get; set; }
		public int PolysDrawn { get; set; } = 0;

        private GraphicsDeviceManager _gdm;
		private GraphicsDevice _graphics;
		private SpriteBatch _spriteBatch;
		private BaseShader _shader;
		private MobModelRender _mobModelRender;

		private float MaskedAlpha = 0.75f;
		private float TransparentAlpha = 0.1f;

		public Render(Game game, int resolutionWidth, int resolutionHeight, bool isFullScreen)
		{
			Width = resolutionWidth;
			Height = resolutionHeight;

			_gdm = new GraphicsDeviceManager(game);
			_gdm.SynchronizeWithVerticalRetrace = false;
			_gdm.PreferredBackBufferWidth = resolutionWidth;
			_gdm.PreferredBackBufferHeight = resolutionHeight;
			_gdm.PreferredDepthStencilFormat = DepthFormat.None;
			_gdm.GraphicsProfile = GraphicsProfile.HiDef;
			_gdm.PreferMultiSampling = false;
			
			if (isFullScreen)
				_gdm.ToggleFullScreen();

			_gdm.ApplyChanges();		
			_mobModelRender = new MobModelRender(this);
		}

		public void Init()
		{
			_graphics = _gdm.GraphicsDevice;
			_spriteBatch = new SpriteBatch(_gdm.GraphicsDevice);
			Camera = new Camera(_graphics);
		}

		public void StartScene()
		{
			StartScene(Color.Transparent);
		}

		public void StartScene(Color bgColor)
		{
			// Setup the graphics device.
			_graphics.SamplerStates[0] = SamplerState.AnisotropicWrap;
			_graphics.BlendState = BlendState.Opaque; 
			_graphics.DepthStencilState = DepthStencilState.Default;

			// Clear the background.
			if (bgColor != Color.Transparent)
				_graphics.Clear(bgColor);
		}

		public void EndScene()
		{
		}

		public void DrawMobModel(MobModel model, GameTime time, Matrix transform)
		{
			_mobModelRender.Draw(model, time, transform);
			LastMeshHasTransparentTextures = _mobModelRender.MobHasTransparentTextures;
		}

		public void DrawPoly(VertexBuffer vertexBuffer)
		{
			_graphics.SetVertexBuffer(vertexBuffer);
			_graphics.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
		}

		public void DrawShape(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, Texture2D texture, int polyCount)
		{
			_shader.SetTexture(texture);

			_graphics.SetVertexBuffer(vertexBuffer);
			_graphics.Indices = indexBuffer;

			_graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, polyCount);
		}

		public void DrawCube(UnitCube cube, Vector3 position, float scale)
		{
			_shader.Effect.Parameters["World"].SetValue(Matrix.CreateScale(scale) * Matrix.CreateTranslation(position));
			_shader.Effect.CurrentTechnique.Passes[0].Apply();

			// Setup the graphics device.
			_graphics.SamplerStates[0] = SamplerState.AnisotropicWrap;
			_graphics.BlendState = BlendState.Opaque;
			_graphics.DepthStencilState = DepthStencilState.Default;
			
			_graphics.SetVertexBuffer(cube.VertexBuffer);
			_graphics.Indices = cube.IndexBuffer;

			_graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
		}

		public void DrawMesh(Mesh mesh, GameTime time)
		{
			DrawMesh(mesh, mesh.VertexBuffer, mesh.IndexBuffer, time);
		}

		public void DrawMesh(Mesh mesh, VertexBuffer vertexBuffer, IndexBuffer indexBuffer, GameTime time)
		{
			if (mesh.Polygons == null)
				return;

			if (UpdateTransformForMesh)
			{
				_shader.World = mesh.Transform;
				_shader.RecalculateFog();
			}

			LastMeshHasTransparentTextures = false;

			// Set buffers.
			_graphics.SetVertexBuffer(vertexBuffer);
			_graphics.Indices = indexBuffer;

			int vertIndex = 0;
			TextureImage lastTexture = null;
			
			foreach (var poly in mesh.Polygons)
			{
				// Get the texture to be drawn.
				if (mesh.Textures[poly.TextureIndex] == null || mesh.Textures[poly.TextureIndex].Image == null)
				{
					vertIndex += poly.PolyCount * 3;
					continue;
				}

				TextureImage texture = mesh.Textures[poly.TextureIndex].Image;

				if (TransparencyRenderMode == TransparencyRenderMode.Opaque && texture.IsMaskedOrTransparent)
				{
					LastMeshHasTransparentTextures = true;
					vertIndex += poly.PolyCount * 3;
					continue;
				}
				else if (TransparencyRenderMode == TransparencyRenderMode.Transparent && !texture.IsMaskedOrTransparent)
				{
					vertIndex += poly.PolyCount * 3;
					continue;
				}

				if (texture != lastTexture)
				{
					if (texture.IsMaskedOrTransparent)
					{
						if (texture.Properties == TextureProperty.Masked && _shader.AlphaTestThreshold != MaskedAlpha)
						{
							_shader.AlphaTestThreshold = MaskedAlpha;
							_shader.ApplyChanges();
						}
						else if (texture.Properties == TextureProperty.Transparent && _shader.AlphaTestThreshold != TransparentAlpha)
						{
							_shader.AlphaTestThreshold = TransparentAlpha;
							_shader.ApplyChanges();
						}
					}

					_shader.SetTexture(texture.Image);
					lastTexture = texture;
				}

				_graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, vertIndex, poly.PolyCount);
				vertIndex += poly.PolyCount * 3;
				PolysDrawn += poly.PolyCount;
			}
		}

		public void DrawMeshFast(Mesh mesh, GameTime time)
		{
			if (mesh.Polygons == null)
				return;

			int polyCount = 0;

			foreach (var poly in mesh.Polygons)
				polyCount += poly.PolyCount;

			if (UpdateTransformForMesh)
			{
				_shader.World = mesh.Transform;
				_shader.RecalculateFog();
			}

			// Set buffers.
			_graphics.Indices = mesh.IndexBuffer;
			_graphics.SetVertexBuffer(mesh.VertexBuffer);

			int vertIndex = 0;

			int skipped = 0;

			foreach (var poly in mesh.Polygons)
			{
				if (mesh.Textures[poly.TextureIndex] == null || mesh.Textures[poly.TextureIndex].Image == null)
				{
					vertIndex += poly.PolyCount * 3;
					skipped++;
					continue;
				}

				_graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, vertIndex, poly.PolyCount);
				vertIndex += poly.PolyCount * 3;
				PolysDrawn += poly.PolyCount;
			}

			mesh.RenderCount++;
		}

		public void DrawSkyBox(SkyBox skybox, GameTime time)
		{
			_graphics.DepthStencilState = DepthStencilState.None;
			DrawMesh(skybox.CurrentSky, time);
			_graphics.DepthStencilState = DepthStencilState.Default;
		}

		public void StartDraw2D()
		{
			_spriteBatch.Begin();
			_graphics.BlendFactor = Color.Black;
		}

		public void EndDraw2D()
		{
			_spriteBatch.End();
		}

		public void DrawImage(Texture2D texture, Rectangle pos, Color tint)
		{
			_spriteBatch.Draw(texture, pos, tint);
		}

		public void DrawImage(Texture2D texture, Vector2 pos, Color tint)
		{
			_spriteBatch.Draw(texture, pos, tint);
		}

		public void DrawImageCropped(Texture2D texture, Rectangle dest, Rectangle source, Color tint)
		{
			_spriteBatch.Draw(texture, dest, source, tint);
		}

		public void DrawText(SpriteFont font, string text, Vector2 pos, Color color)
		{
			_spriteBatch.DrawString(font, text, pos, color);
		}

		public void Dispose()
		{
			RenderUtil.DisposeIfNotNull(_shader);
			_shader = null;

			RenderUtil.DisposeIfNotNull(_spriteBatch);
			_spriteBatch = null;
		}
		
		public void UpdateCamera()
		{
			if (Camera != null)
				Camera.Update();

			if (Camera != null && _shader != null)
			{
				_shader.World = Camera.World;
				_shader.View = Camera.View;
				_shader.Projection = Camera.Projection;
				_shader.RecalculateFog();
			}
		}

		private void ChangeShader(BaseShader shader)
		{
			_shader = shader;
			UpdateCamera();
		}
	}
}
