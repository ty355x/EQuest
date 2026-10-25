using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;

namespace EQuestClient.Game
{
	public class MobTitle : IDisposable
	{
		private Mob _mob;
		private VertexBuffer _vertexBuffer;
		private IndexBuffer _indexBuffer;
		private Texture2D _texture;
		private float _scale = 0.05f;
		private float _width;
		private float _height;

		public MobTitle(Mob mob, SpriteFont font, Texture2D texture, Render render)
		{
			_mob = mob;
			MakeTexture(render, font, _mob.Name);
			BuildTitlePolygon(render.Graphics);
		}

		public void Draw(Render render, Player player)
		{
			// Find the center of the title.
			Vector3 titleCenter = new Vector3(_width * _scale / 2, 0, 0);

			// Create the matrix to rotate the title towards the player.
			Matrix rotate = Matrix.CreateRotationZ(MathHelper.ToRadians(player.Heading - 180));

			// Rotate the center.
			titleCenter = Vector3.Transform(titleCenter, rotate);

			// Create a vector to translate the title over the mob.
			Vector3 translate = new Vector3(
				_mob.Position.X - titleCenter.X, 
				_mob.Position.Y - titleCenter.Y, 
				_mob.Position.Z + (float)((_mob.Height + 1.75) * _mob.RaceSize));

			// Create the transformation matrix.
			Matrix transform =
				rotate *
				Matrix.CreateScale(_scale) *
				Matrix.CreateTranslation(translate);

			// Draw the title.
			render.Camera.World = transform;
			render.UpdateCamera();
			
			render.DrawShape(_vertexBuffer, _indexBuffer, _texture, 2);
		}

		public void Dispose()
		{
			RenderUtil.DisposeIfNotNull(_vertexBuffer);
			_vertexBuffer = null;

			RenderUtil.DisposeIfNotNull(_indexBuffer);
			_indexBuffer = null;
		}

		private void BuildTitlePolygon(GraphicsDevice graphics)
		{
			VertexPositionTexture[] vertices = new VertexPositionTexture[4];
			
			vertices[0] = new VertexPositionTexture(
				new Vector3(0, 0, 0), new Vector2(0, 0));
			vertices[1] = new VertexPositionTexture(
				new Vector3(_width, 0, 0), new Vector2(1, 0));
			vertices[2] = new VertexPositionTexture(
				new Vector3(_width, 0, -_height), new Vector2(1, 1));
			vertices[3] = new VertexPositionTexture(
				new Vector3(0, 0, -_height), new Vector2(0, 1));

			_vertexBuffer = new VertexBuffer(graphics, typeof(VertexPositionTexture), vertices.Length, BufferUsage.WriteOnly);
			_vertexBuffer.SetData<VertexPositionTexture>(vertices);

			short[] indices = new short[] { 0, 1, 2, 0, 2, 3 };
			_indexBuffer = new IndexBuffer(graphics, typeof(short), indices.Length, BufferUsage.WriteOnly);
			_indexBuffer.SetData<short>(indices);
		}

		private void MakeTexture(Render render, SpriteFont font, string text)
		{
			GraphicsDevice graphics = render.Graphics;

			Vector2 size = font.MeasureString(text);
			_width = size.X;
			_height = size.Y;
			
			RenderTarget2D renderTarget = new RenderTarget2D(graphics, (int)_width + 2, (int)_height + 3);
			graphics.SetRenderTarget(renderTarget);
			graphics.Clear(Color.Transparent);

			Color outlineColor = new Color(16, 16, 16);
			Color titleColor = Color.CornflowerBlue;

			render.StartDraw2D();
			render.DrawText(font, text, new Vector2(1, 2), outlineColor);
			render.DrawText(font, text, new Vector2(2, 1), titleColor);
			render.EndDraw2D();

			graphics.SetRenderTarget(null);
			_texture = (Texture2D)renderTarget;
		}
	}
}
