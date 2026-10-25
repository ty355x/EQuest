using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using S3DFileLib;
using EQuestLib;
using EQuestLib.Octrees;
using EQuestRender;
using EQuestRender.Readers;
using Mesh = EQuestRender.Mesh;

namespace EQuestClient.Game
{
	public class Zone : IDisposable
	{
		private GameScene _scene;
		private Render _render;
		private TextureListContainer _terrainTextures;
		private TextureListContainer _objectTextures;

		public List<ZoneTerrain> Terrain { get; private set; }
		public List<ZoneObject> Objects { get; private set; }
		public List<Mob> Mobs { get; private set; }
		public BspTree BspTree { get; private set; }
		public Octree Octree { get; private set; }
		public bool DrawMesh { get; set; }

		public Zone(string gamePath, string zoneName, GameScene scene, Render render)
		{
			_scene = scene;
			_render = render;
			Mobs = new List<Mob>(); 

			LoadZoneData(gamePath, zoneName, render.Graphics);
			
			var treeBuilder = new OctreeBuilder();
			Octree = treeBuilder.MakeOctee(Terrain, Objects);
		}

		public string DetermineMobConsoleName(int mobId)
		{
			if (mobId < 0)
				return "Unknown";

			if (mobId == _scene.Player.MobId)
				return "You";

			var mob = FindMob(mobId);
			return mob != null ? mob.Name : "Unknown";		
		}

		public void AddMob(Mob mob)
		{
			if (mob.IsCurrentPlayer)
				return;

			Mobs.Add(mob);

			if (mob.Type == MobType.Player)
				_scene.UI.Write(mob.Name + " has entered the zone.");
		}

		public void RemoveMob(int mobId)
		{
			var mob = FindMob(mobId);

			if (mob != null)
			{
				Mobs.Remove(mob);

				if (mob.Type == MobType.Player)
					_scene.UI.Write(mob.Name + " has left the zone.");

				if (_scene.Player.Target == mob)
					_scene.Player.Target = null;
			}
		}

		public void UpdateMobMovement(int mobId, Vector3 position, float speed, float heading, int rotateDirection)
		{
			var mob = FindMob(mobId);

			if (mob != null)
				mob.SetNewFinalPosition(position, speed, heading, rotateDirection);
		}

		public void Update(GameTime time, Render render)
		{
			if (_terrainTextures != null)
				_terrainTextures.Update(time);

			if (_objectTextures != null)
				_objectTextures.Update(time);

			if (Terrain != null)
			{
				foreach (var terrainModel in Terrain)
					terrainModel.Mesh.Update(time);
			}

			if (Objects != null)
			{
				foreach (var zoneObject in Objects)
					zoneObject.Mesh.Update(time);
			}

			if (Mobs != null)
			{
				foreach (var mob in Mobs)
					mob.Update(time);
			}
		}

		public Mob FindMob(int mobId)
		{
			if (mobId == _scene.Player.MobId)
				return _scene.Player.Mob;

			return Mobs.Where(m => m.ID == mobId).FirstOrDefault();
		}

		public void Dispose()
		{
			RenderUtil.Dispose(Terrain);
			Terrain = null;

			RenderUtil.DisposeIfNotNull(_terrainTextures);
			_terrainTextures = null;

			RenderUtil.Dispose(Objects);
			Objects = null;

			RenderUtil.DisposeIfNotNull(_objectTextures);
			_objectTextures = null;

			Octree = null;
		}

		private void LoadZoneData(string assetsPath, string zoneName, GraphicsDevice graphics)
		{
			string s3dPath = Path.Combine(assetsPath);
			string zoneS3dPath = Path.Combine(s3dPath, zoneName + ".s3d");

			List<Mesh> terrainModels;

			using (S3DFile s3dZone = S3DFile.Open(zoneS3dPath))
			using (WLDFile wldZone = s3dZone.GetWldFile(zoneName + ".wld"))
			{
				// Load the zone.
				ModelReader modelReader = new ModelReader();
				modelReader.ReadModels(s3dZone, wldZone, graphics, out terrainModels, out _terrainTextures);

				Terrain = new List<ZoneTerrain>();

				foreach (Mesh mesh in terrainModels)
					Terrain.Add(new ZoneTerrain(mesh));
				
				// Load the BSP tree.
				BspTreeReader treeReader = new BspTreeReader();
				BspTree = treeReader.ReadBspTree(wldZone);

				// Load the zone objects.
				string pathObjS3d = Path.Combine(s3dPath, zoneName + "_obj.s3d");

				using (S3DFile s3dObjects = S3DFile.Open(pathObjS3d))
				using (WLDFile wldObjects = s3dObjects.GetWldFile(zoneName + "_obj.wld"))
				using (WLDFile wldObjPos = s3dZone.GetWldFile("objects.wld"))
				{
					List<ZoneObject> objects;
					ModelReader reader = new ModelReader();
					reader.ReadZoneObjects(s3dObjects, wldObjects, wldObjPos, out objects, out _objectTextures,
						graphics);
					Objects = objects;
				}
			}

			AssignMeshesToBspRegions();
		}

		private void AssignMeshesToBspRegions()
		{
			var terrainHash = new Dictionary<string, ZoneTerrain>();

			foreach (ZoneTerrain terrain in Terrain)
				terrainHash.Add(terrain.Mesh.Name, terrain);

			AssignMeshToBspRegionAndChildren(BspTree.Root, terrainHash);
		}

		private void AssignMeshToBspRegionAndChildren(BspNode node, Dictionary<string, ZoneTerrain> meshHash)
		{
			if (node.Region != null && meshHash.ContainsKey(node.Region.MeshName))
				node.Region.Terrain = meshHash[node.Region.MeshName];
			
			if (node.FrontNode != null)
				AssignMeshToBspRegionAndChildren(node.FrontNode, meshHash);

			if (node.BackNode != null)
				AssignMeshToBspRegionAndChildren(node.BackNode, meshHash);
		}
	}
}
