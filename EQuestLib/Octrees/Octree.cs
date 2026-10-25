using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using EQuestRender;
using CollisionHelper = EQuestLib.Collisions.Collision;

namespace EQuestLib.Octrees
{
	public class Octree
	{
		public OctreeNode Root;
		public OctreeCollision Collision { get; private set; }
		private const int MaxCollisionNodes = 256;
		
		public Octree()
		{
			Collision = new OctreeCollision(this, MaxCollisionNodes);
		}

		public void AddTerrainMesh(ZoneTerrain entry)
		{
			AddTerrainMeshToNode(Root, entry);
		}

		public void AddZoneObject(ZoneObject obj)
		{
			AddZoneObjectToNode(Root, obj);
		}

		private void AddTerrainMeshToNode(OctreeNode node, ZoneTerrain terrain)
		{
			if (CollisionHelper.BoxCollidesWithMeshBounds(node.Bounds, terrain.Mesh))
			{
				if (node.IsLeaf)
				{
					node.Terrain.Add(terrain);
				}
				else
				{
					foreach (var child in node.Children)
					{
						AddTerrainMeshToNode(child, terrain);
					}
				}
			}
		}

		private void AddZoneObjectToNode(OctreeNode node, ZoneObject obj)
		{
			if (node.Bounds.Contains(obj.Bounds) != ContainmentType.Disjoint)
			{
				if (node.IsLeaf)
				{
					node.ZoneObjects.Add(obj);
				}
				else
				{
					foreach (var child in node.Children)
					{
						AddZoneObjectToNode(child, obj);
					}
				}
			}
		}
	}
}
