using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestClient.AssetManagement;
using EQuestRender;
using EQuestRender.Shaders;

namespace EQuestClient.Game
{
	public class GameSceneOcclusionRender
	{
		private Render _render;
		private RenderTarget2D _renderTarget;
		private Player _player;

		private FastShader _fastShader;
		private ZoneTerrain _lastDrawnTerrain;

		private Matrix _terrainTransform;

		public GameSceneOcclusionRender(AssetManager assets, Player player, Render render)
		{
			_player = player;
			_render = render;
			_renderTarget = new RenderTarget2D(_render.Graphics, _render.Graphics.Viewport.Width, _render.Graphics.Viewport.Height);
			_terrainTransform = Matrix.CreateScale(1);

			_fastShader = new FastShader(assets.LoadAsset<Effect>(@"Shaders\BasicShader"));
			_fastShader.SurfaceColor = Color.Black;
		}
		
		public void Draw(Zone zone, GameTime time)
		{
			foreach (ZoneTerrain terrain in zone.Terrain)
				terrain.Mesh.RenderCount = 0;
			
			_render.Graphics.BlendState = BlendState.Opaque;	
			_render.PolysDrawn = 0;

			//_render.Shader = _fastShader;
			_render.TransparencyRenderMode = TransparencyRenderMode.Opaque;
			_render.Camera.World = _terrainTransform;
			_render.UpdateCamera();
			_render.Shader.ApplyChanges();

			_render.UpdateTransformForMesh = false;
			//RenderLog.Debug("Starting frame");
			DrawTerrainNodeRecursive(zone.BspTree.Root, time);
			//RenderLog.Debug("Ending frame");
			
			_render.UpdateTransformForMesh = true;
			
			// RenderLog.Debug("Rendered " + _render.TotalPolys + " total polys");

			// Draw zone objects.
			/*if (zone.Objects != null)
			{
				foreach (ZoneObject obj in zone.Objects)
				{
					if (obj.ShouldDraw && !obj.Occlusion.IsOccluded)
					{
						obj.Mesh.Transform = obj.Transform;
						obj.Occlusion.BeginQuery();
						_render.DrawMesh(obj.Mesh, time);
						obj.Occlusion.EndQuery();

						obj.Flag = _render.LastMeshHasTransparentTextures;
						_lastDrawnObject = obj;
					}
				}
			}*/

			// Draw mobs.
			/*if (zone.Mobs != null)
			{
				foreach (Mob mob in zone.Mobs)
				{
					if (mob.ShouldDraw && !mob.Occlusion.IsOccluded)
					{
						mob.Occlusion.BeginQuery();
						mob.Draw(_render, time, _player, false, false);
						mob.Occlusion.EndQuery();
						_lastDrawnMob = mob;
					}
				}
			}*/
		}

		public void DrawTerrainNodeRecursive(BspNode node, GameTime time)
		{
			const int MaxPolys = 1500;
			
			PlaneIntersectionType intersect = node.Plane.Intersects(_player.BoundingBox);

			if (intersect == PlaneIntersectionType.Front)
			{
				if (node.FrontNode != null)
					DrawTerrainNodeRecursive(node.FrontNode, time);

				if (node.Region != null && node.Region.Terrain != null && node.Region.Terrain.InsideFrustrum)
					DrawTerrainMesh(node.Region.Terrain, time);

				if (_render.PolysDrawn > MaxPolys)
					return;
				
				if (node.BackNode != null)
					DrawTerrainNodeRecursive(node.BackNode, time);
			}
			else
			{
				if (node.BackNode != null)
					DrawTerrainNodeRecursive(node.BackNode, time);

				if (node.Region != null && node.Region.Terrain != null && node.Region.Terrain.InsideFrustrum)
					DrawTerrainMesh(node.Region.Terrain, time);

				if (_render.PolysDrawn > MaxPolys)
					return;

				if (node.FrontNode != null)
					DrawTerrainNodeRecursive(node.FrontNode, time);
			}
		}

		public void DrawTerrainMesh(ZoneTerrain terrain, GameTime time)
		{
			terrain.Occlusion.BeginQuery();
			_render.DrawMesh(terrain.Mesh, time);
			terrain.Occlusion.EndQuery();
			terrain.HasTransparentTextures = _render.LastMeshHasTransparentTextures;
			_lastDrawnTerrain = terrain;
		}
	}
}
