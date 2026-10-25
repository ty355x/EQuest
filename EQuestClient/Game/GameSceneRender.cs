using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestClient.AssetManagement;
using EQuestClient.Game.UI;
using EQuestRender;
using EQuestRender.Shaders;

namespace EQuestClient.Game
{
	public class GameSceneRender
	{
		private const int ObjectDrawDistance = 5000 * 5000;
		private const int MobDrawDistance = 1200 * 1200;
		private const int MobTitleDrawDistance = 400 * 400;
		private const int SmallObjectDrawDistance = 500 * 500;
		private const string DebugPosFormat = "POS: ({0:0.00}, {1:0.00}, {2:0.00}), Heading: {3:0.00}";
		private const int OcclusionThreshold = 5;
		private GameSceneOcclusionRender _occlusionRender;

		private AssetManager _assets;
		private Player _player;
		private Render _render;
		private GameSceneUI _ui;

		private string _fps = "";
		private float _lastFPSUpdate = 0f;
		private int _frameCount = 0;

		private bool DrawDebugInfo = true;
		private bool DrawDebugExtra = false;
		
		private WorldShader _worldShader;
		private BasicEffectShader _basicShader;
		private Matrix _skyboxTransform;
		private Matrix _terrainTransform;
		
		private ZoneTerrain _lastDrawnTerrain;
		private ZoneObject _lastDrawnObject = null;
		private Mob _lastDrawnMob = null;

		public GameSceneRender(AssetManager assets, Player player, Render render, GameSceneUI ui)
		{
			_assets = assets;
			_player = player;
			_render = render;
			_ui = ui;
			_occlusionRender = new GameSceneOcclusionRender(assets, _player, _render);

			_worldShader = new WorldShader(assets.LoadAsset<Effect>(@"Shaders\WorldShader"));
			_worldShader.AlphaTestThreshold = 0.1f;
			_worldShader.FogEnabled = true;
			ApplyFog(new Color(0.5f, 0.5f, 0.5f, 1.0f), 500, 1800);

			_basicShader = new BasicEffectShader(render.Graphics);

			_skyboxTransform = Matrix.CreateScale(100);
			_terrainTransform = Matrix.CreateScale(1);
		}

		public void DoPreRenderPass(Zone zone, bool doTerrainOcclusion, bool doObjectOcclusion)
		{
			_render.Camera.Position = _player.Camera.Position;
			_render.Camera.Rotation = _player.Rotation;
			_render.Camera.World = Matrix.CreateScale(1.0f);
			_render.UpdateCamera();

			if (zone.Terrain != null)
			{
				foreach (ZoneTerrain terrain in zone.Terrain)
				{
					terrain.InsideFrustrum = _render.Camera.ContainsBox(terrain.Mesh.CalculatedBounds);

					if (doTerrainOcclusion)
						terrain.Occlusion.IsOccluded = false;
				}
			}

			if (zone.Objects != null)
			{
				foreach (ZoneObject obj in zone.Objects)
				{
					obj.ShouldDraw = ShouldDrawObject(obj);

					if (doObjectOcclusion)
						obj.Occlusion.IsOccluded = false;
				}
			}

			if (zone.Mobs != null)
			{
				foreach (Mob mob in zone.Mobs)
				{
					CheckShouldDrawMob(mob);

					if (doObjectOcclusion)
						mob.Occlusion.IsOccluded = false;
				}
			}
		}

		public void DrawScene(Zone zone, GameTime time, bool useVr = false)
		{
			PrepareOcclusionTargets(zone);

			bool doTerrainOcclusion = (_frameCount == 0 || _frameCount == 2);
			bool doObjectOcclusion = _frameCount == 0;

			DoPreRenderPass(zone, doTerrainOcclusion, doObjectOcclusion);

			_render.Shader = _worldShader;
			_render.Camera.Position = _player.Camera.Position;
			_render.Camera.Rotation = _player.Rotation;
			_render.UpdateCamera();

			_render.StartScene(Color.CornflowerBlue);
			_occlusionRender.Draw(zone, time);
			_render.EndScene();

			DrawUI(time);

			return;
			
			if (!doObjectOcclusion)
				WaitForOcclusionToTerminate(zone);

			// _render.SetupVRCamera(matrices.View, matrices.Project);
			_render.StartScene(Color.Transparent);
			DrawSkybox(time, useVr);

			_render.Shader = _worldShader;
			_render.Camera.Position = _player.Camera.Position;
			_render.Camera.Rotation = _player.Rotation;
			_render.UpdateCamera();

			foreach (EffectPass pass in _render.Shader.Effect.CurrentTechnique.Passes)
			{
				DrawWorld(zone, time, doTerrainOcclusion, doObjectOcclusion, false);
				DrawTransparentMeshes(zone, time);
			}

			DrawUI(time);
			_frameCount = _frameCount == 3 ? 0 : _frameCount + 1;
		}

		private void WaitForOcclusionToTerminate(Zone zone)
		{
			if (_lastDrawnTerrain != null)
			{
				_lastDrawnTerrain.Occlusion.WaitForCompletion();

				foreach (ZoneTerrain terrain in zone.Terrain)
					terrain.Occlusion.ProcessResult();

				_lastDrawnTerrain = null;
			}

			if (_lastDrawnObject != null)
			{
				_lastDrawnObject.Occlusion.WaitForCompletion();

				foreach (ZoneObject obj in zone.Objects)
					obj.Occlusion.ProcessResult();

				_lastDrawnObject = null;
			}

			if (_lastDrawnMob != null)
			{
				_lastDrawnMob.Occlusion.WaitForCompletion();

				foreach (Mob mob in zone.Mobs)
					mob.Occlusion.ProcessResult();

				_lastDrawnMob = null;
			}
		}

		public void CheckTerrainNodeVisibleRecursive(BspNode node, BspNode parentNode)
		{
			PlaneIntersectionType intersect = node.Plane.Intersects(_player.BoundingBox);

			if (intersect == PlaneIntersectionType.Front)
			{
				if (node.FrontNode != null)
					CheckTerrainNodeVisibleRecursive(node.FrontNode, node);

				if (node.Region != null && node.Region.Terrain != null && node.Region.Terrain.InsideFrustrum)
				{
					if (parentNode != null)
						parentNode.Region.Terrain.Next = node.Region.Terrain;
				}

				if (node.BackNode != null)
					CheckTerrainNodeVisibleRecursive(node.BackNode, node);
			}
			else
			{
				if (node.BackNode != null)
					CheckTerrainNodeVisibleRecursive(node.BackNode, node);

				if (node.Region != null && node.Region.Terrain != null && node.Region.Terrain.InsideFrustrum)
				{
					if (parentNode != null)
						parentNode.Region.Terrain.Next = node.Region.Terrain;
				}

				if (node.FrontNode != null)
					CheckTerrainNodeVisibleRecursive(node.FrontNode, node);
			}
		}

		public void DrawTerrainNodeRecursive(BspNode node, bool runOcclusionQuery, GameTime time, bool useVr = false)
		{
			PlaneIntersectionType intersect = node.Plane.Intersects(_player.BoundingBox);

			if (intersect == PlaneIntersectionType.Front)
			{
				if (node.FrontNode != null)
					DrawTerrainNodeRecursive(node.FrontNode, runOcclusionQuery, time, useVr);

				if (node.Region != null && node.Region.Terrain != null && node.Region.Terrain.InsideFrustrum)
					DrawTerrainMesh(node.Region.Terrain, runOcclusionQuery, time);

				if (node.BackNode != null)
					DrawTerrainNodeRecursive(node.BackNode, runOcclusionQuery, time, useVr);
			}
			else
			{
				if (node.BackNode != null)
					DrawTerrainNodeRecursive(node.BackNode, runOcclusionQuery, time, useVr);

				if (node.Region != null && node.Region.Terrain != null && node.Region.Terrain.InsideFrustrum)
					DrawTerrainMesh(node.Region.Terrain, runOcclusionQuery, time);

				if (node.FrontNode != null)
					DrawTerrainNodeRecursive(node.FrontNode, runOcclusionQuery, time, useVr);
			}
		}

		public void DrawTerrainMesh(ZoneTerrain terrain, bool runOcclusionQuery, GameTime time)
		{
			if (!runOcclusionQuery && terrain.Occlusion.IsOccluded)
				return;

			if (runOcclusionQuery)
			{
				terrain.Occlusion.BeginQuery();
				_render.DrawMesh(terrain.Mesh, time);
				terrain.Occlusion.EndQuery();
			}
			else
			{
				_render.DrawMesh(terrain.Mesh, time);
			}	

			terrain.HasTransparentTextures = _render.LastMeshHasTransparentTextures;
			_lastDrawnTerrain = terrain;
		}

		public void ApplyFog(Color color, float fogStart, float fogEnd)
		{
			_render.Camera.FarPlane = fogEnd;
			_worldShader.FogColor = color;
			_worldShader.FogEnabled = true;
			_worldShader.ApplyFog(fogStart, fogEnd);
		}

		public void DrawPlayerOctreeNodeContents(Zone zone, GameTime time, bool drawTransparent, bool useVr, Matrix? view, Matrix? projection)
		{
			zone.Octree.Collision.FindNodesBoxCollidesWith(_player.BoundingBox);

			for (int i = 0; i < zone.Octree.Collision.NodeCount; i++)
			{
				var node = zone.Octree.Collision.Nodes[i];

				foreach (ZoneTerrain terrain in node.Terrain)
					_render.DrawMesh(terrain.Mesh, time);

				foreach (var obj in node.ZoneObjects)
				{
					_render.Camera.World = obj.Transform;
					_render.UpdateCamera();
					_render.DrawMesh(obj.Mesh, time);
				}
			}
		}

		private void PrepareOcclusionTargets(Zone zone)
		{
			foreach (ZoneTerrain terrain in zone.Terrain)
			{
				if (terrain.Occlusion == null)
					terrain.Occlusion = new OcclusionTracker(_render.Graphics, OcclusionThreshold);
			}

			foreach (ZoneObject obj in zone.Objects)
			{
				if (obj.Occlusion == null)
					obj.Occlusion = new OcclusionTracker(_render.Graphics, OcclusionThreshold);
			}

			foreach (Mob mob in zone.Mobs)
			{
				if (mob.Occlusion == null)
					mob.Occlusion = new OcclusionTracker(_render.Graphics, OcclusionThreshold);
			}
		}

		private void DrawSkybox(GameTime time, bool useVr)
		{
			if (_assets.SkyBox == null)
				return;

			_render.Shader = _worldShader;
			_render.TransparencyRenderMode = TransparencyRenderMode.All;
			_render.Camera.Rotation = _player.Rotation;
			_render.Camera.Position = Vector3.Zero;
			_render.Camera.World = _skyboxTransform;
			_render.UpdateTransformForMesh = false;
			_render.UpdateCamera();
			_render.Shader.ApplyChanges();

			foreach (EffectPass pass in _render.Shader.Effect.CurrentTechnique.Passes)
				_render.DrawSkyBox(_assets.SkyBox, time);
		}

		private void DrawWorld(Zone zone, GameTime time, bool doTerrainOcclusion, bool doObjectOcclusion, bool useVr)
		{
			_render.TransparencyRenderMode = TransparencyRenderMode.Opaque;
			_render.Camera.World = _terrainTransform;
			_render.UpdateCamera();
			
			_render.UpdateTransformForMesh = false;
			DrawTerrainNodeRecursive(zone.BspTree.Root, doTerrainOcclusion, time);

			_render.UpdateTransformForMesh = true;

			// Draw zone objects.
			if (zone.Objects != null)
			{
				foreach (ZoneObject obj in zone.Objects)
				{
					if (obj.ShouldDraw && !obj.Occlusion.IsOccluded)
					{
						obj.Mesh.Transform = obj.Transform;

						if (doObjectOcclusion)
							obj.Occlusion.BeginQuery();

						_render.DrawMesh(obj.Mesh, time);

						if (doObjectOcclusion)
							obj.Occlusion.EndQuery();
						
						obj.Flag = _render.LastMeshHasTransparentTextures;
						_lastDrawnObject = obj;
					}
				}
			}

			// Draw mobs.
			if (zone.Mobs != null)
			{
				foreach (Mob mob in zone.Mobs)
				{
					if (mob.ShouldDraw && !mob.Occlusion.IsOccluded)
					{
						if (doObjectOcclusion)
							mob.Occlusion.BeginQuery();

						mob.Draw(_render, time, _player, mob.ShouldDrawTitle, DrawDebugExtra);

						if (doObjectOcclusion)
							mob.Occlusion.EndQuery();

						_lastDrawnMob = mob;
					}
				}
			}
		}

		private void DrawTransparentMeshes(Zone zone, GameTime time)
		{
			_render.Camera.World = _terrainTransform;
			_render.UpdateCamera();
			_render.UpdateTransformForMesh = false;
			_render.TransparencyRenderMode = TransparencyRenderMode.Transparent;

			// Draw terrain.
			if (zone.Terrain != null)
			{
				foreach (ZoneTerrain terrain in zone.Terrain)
				{
					if (terrain.HasTransparentTextures)
					{
						_render.DrawMesh(terrain.Mesh, time);
						terrain.HasTransparentTextures = false;
					}
				}
			}

			_render.UpdateTransformForMesh = true;

			// Draw zone objects.
			if (zone.Objects != null)
			{
				foreach (var obj in zone.Objects)
				{
					if (obj.Flag)
					{
						obj.Mesh.Transform = obj.Transform;
						_render.DrawMesh(obj.Mesh, time);
						obj.Flag = false;
					}
				}
			}

			// Draw mobs.
			if (zone.Mobs != null)
			{
				foreach (var mob in zone.Mobs)
				{
					if (mob.Flag)
					{
						mob.Draw(_render, time, _player, false, false);
						mob.Flag = false;
					}
				}
			}
		}

		private void DrawUI(GameTime time)
		{
			_render.StartDraw2D();

			// Draw the UI.
			if (_ui != null)
				_ui.Draw(_render, time);

			// Draw the cursor item.
			if (_player.Cursor.HasItems())
				_player.Cursor.DrawItem(_render);

			if (DrawDebugInfo)
				DrawDebug(time);

			_render.EndDraw2D();
		}

		private bool ShouldDrawObject(ZoneObject obj)
		{
			float distanceSq = Math.Abs(Vector3.DistanceSquared(_player.Camera.Position, obj.Position));

			if (_render.Camera.Frustrum.Contains(obj.Bounds) == ContainmentType.Disjoint ||
				distanceSq >= ObjectDrawDistance)
			{
				return false;
			}

			if (obj.IsSmall && distanceSq >= SmallObjectDrawDistance)
				return false;

			return true;
		}

		private void CheckShouldDrawMob(Mob mob)
		{
			mob.ShouldDraw = true;
			
			if (mob.IsCurrentPlayer || !mob.IsVisible ||
				_render.Camera.Frustrum.Contains(mob.BoundingBox.Bounds) == ContainmentType.Disjoint)
			{
				mob.ShouldDraw = false;
				return;
			}

			float mobDistance = Vector3.DistanceSquared(_player.Camera.Position, mob.Position);

			if (mobDistance >= MobDrawDistance)
				return;

			mob.ShouldDraw = true;
			mob.ShouldDrawTitle = mobDistance <= MobTitleDrawDistance;
		}

		private void DrawDebug(GameTime time)
		{
			string pos = string.Format(DebugPosFormat, _player.Body.Position.X,
				_player.Body.Position.Y, _player.Body.Position.Z, _player.Heading);

			_render.DrawText(_assets.UI.DefaultFont, pos, new Vector2(10, 10), Color.Yellow);

			if (time.TotalGameTime.TotalSeconds - _lastFPSUpdate >= 1)
			{
				_fps = Math.Round(1 / time.ElapsedGameTime.TotalSeconds).ToString();
				_lastFPSUpdate = (float)time.TotalGameTime.TotalSeconds;
			}

			_render.DrawText(_assets.UI.DefaultFont, _fps, new Vector2(10, 25), Color.Yellow);
		}
	}
}
