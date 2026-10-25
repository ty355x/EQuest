using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using EQuestRender;
using CollisionHelper = EQuestLib.Collisions.Collision;

namespace EQuestLib.Octrees
{
	public class OctreeNode
	{
		public BoundingBox Bounds { get; private set; }
		public OctreeNode[] Children { get; private set; }
		public List<ZoneTerrain> Terrain { get; private set; }
		public List<ZoneObject> ZoneObjects { get; private set; }
		public bool IsLeaf { get; private set; }

		private OctreeNode(bool isLeaf)
		{
			this.IsLeaf = isLeaf;

			if (IsLeaf)
			{
				Children = null;
				Terrain = new List<ZoneTerrain>();
				ZoneObjects = new List<ZoneObject>();
			}
			else
			{
				Children = new OctreeNode[8];
				Terrain = null;
				ZoneObjects = null;
			}
		}
		
		public OctreeNode(Vector3 min, Vector3 max, bool isLeaf) : this(isLeaf)
		{
			Bounds = new BoundingBox(min, max);
		}

		public OctreeNode(Vector3 min, float xsize, float ysize, float zsize, bool isLeaf) : this(isLeaf)
		{
			Bounds = new BoundingBox(min, new Vector3(min.X + xsize, min.Y + ysize, min.Z + zsize));
		}

		public bool BoxCollidesWithObject(BoundingBox box)
		{
			foreach (var mesh in Terrain)
			{
				if (CollisionHelper.BoxCollidesWithMesh(box, mesh.Mesh))
					return true;
			}

			foreach (var obj in ZoneObjects)
			{
				if (CollisionHelper.BoxCollidesWithZoneObject(box, obj))
					return true;
			}

			return false;
		}

		public PolyCollisionResult FindPolyBoxCollidesWith(BoundingBox box)
		{
			foreach (var mesh in Terrain)
			{
				var poly = CollisionHelper.FindPolyBoxCollidesWith(box, mesh.Mesh);

				if (poly != null)
					return PolyCollisionResult.MakeCollisionResult(poly, CollisionObjectType.Terrain);
			}

			foreach (var obj in ZoneObjects)
			{
				var poly = CollisionHelper.FindZoneObjectPolyBoxCollidesWith(box, obj);

				if (poly != null)
					return PolyCollisionResult.MakeCollisionResult(poly, CollisionObjectType.ZoneObject);
			}

			return PolyCollisionResult.MakeNoCollisionResult();
		}

		public override string ToString()
		{
			return "OctreeNode Min: " + Bounds.Min + " Max: " + Bounds.Max;
		}
	}
}
