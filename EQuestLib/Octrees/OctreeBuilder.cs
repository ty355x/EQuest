using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

using EQuestRender;

namespace EQuestLib.Octrees
{
	public class OctreeBuilder
	{
		private int _nodeCount = 0;
		
		public Octree MakeOctee(List<ZoneTerrain> terrain, List<ZoneObject> zoneObjects)
		{
			Vector3 min = Vector3.Zero;
			Vector3 max = Vector3.Zero;

			CreateMinMaxVectors(terrain, ref min, ref max);
			Octree tree = CreateTreeAndNodes(ref min, ref max);
			
			AddTerrainMeshes(tree, terrain);
			AddZoneObjects(tree, zoneObjects);

			// PrintTreeDebugInfo(tree);

			return tree;
		}

		private void AddTerrainMeshes(Octree tree, List<ZoneTerrain> terrain)
		{
			foreach (ZoneTerrain terrainMesh in terrain)
				tree.AddTerrainMesh(terrainMesh);
		}

		private void CreateMinMaxVectors(List<ZoneTerrain> terrain, ref Vector3 min, ref Vector3 max)
		{
			min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			foreach (var terrainEntry in terrain)
			{
				foreach (var poly in terrainEntry.Mesh.CollisionPolys)
				{
					AssignMin(ref poly.V0, ref min);
					AssignMin(ref poly.V1, ref min);
					AssignMin(ref poly.V2, ref min);

					AssignMax(ref poly.V0, ref max);
					AssignMax(ref poly.V1, ref max);
					AssignMax(ref poly.V2, ref max);
				}
			}

			EnsureValidVector(ref min);
			EnsureValidVector(ref max);
		}

		private void AddZoneObjects(Octree tree, List<ZoneObject> zoneObject)
		{
			foreach (var obj in zoneObject)
			{
				tree.AddZoneObject(obj);
			}
		}

		private Octree CreateTreeAndNodes(ref Vector3 min, ref Vector3 max)
		{
			float leafSize = 100;
			
			var tree = new Octree();
			tree.Root = new OctreeNode(min, max, false);
			_nodeCount = 1;

			AddNodeChildren(tree.Root, leafSize);
			
			return tree;
		}

		private void AddNodeChildren(OctreeNode node, float leafSize)
		{
			var min = node.Bounds.Min;
			var max = node.Bounds.Max;

			float sizeX = (max.X - min.X) / 2;
			float sizeY = (max.Y - min.Y) / 2;
			float sizeZ = (max.Z - min.Z) / 2;

			float midX = min.X + sizeX;
			float midY = min.Y + sizeY;
			float midZ = min.Z + sizeZ;

			bool childIsLeaf = sizeX <= leafSize || sizeY <= leafSize || sizeZ <= leafSize;

			node.Children[0] = new OctreeNode(new Vector3(min.X, min.Y, midZ), sizeX, sizeY, sizeZ, childIsLeaf);
			node.Children[1] = new OctreeNode(new Vector3(min.X, midY, midZ), sizeX, sizeY, sizeZ, childIsLeaf);
			node.Children[2] = new OctreeNode(new Vector3(midX, midY, midZ), sizeX, sizeY, sizeZ, childIsLeaf);
			node.Children[3] = new OctreeNode(new Vector3(midX, min.Y, midZ), sizeX, sizeY, sizeZ, childIsLeaf);
			node.Children[4] = new OctreeNode(new Vector3(min.X, min.Y, min.Z), sizeX, sizeY, sizeZ, childIsLeaf);
			node.Children[5] = new OctreeNode(new Vector3(min.X, midY, min.Z), sizeX, sizeY, sizeZ, childIsLeaf);
			node.Children[6] = new OctreeNode(new Vector3(midX, midY, min.Z), sizeX, sizeY, sizeZ, childIsLeaf);
			node.Children[7] = new OctreeNode(new Vector3(midX, min.Y, min.Z), sizeX, sizeY, sizeZ, childIsLeaf);

			_nodeCount += 8;

			if (!childIsLeaf)
			{
				foreach (var child in node.Children)
					AddNodeChildren(child, leafSize);
			}
		}

		private void AssignMin(ref Vector3 vec, ref Vector3 min)
		{
			if (vec.X < min.X) min.X = vec.X;
			if (vec.Y < min.Y) min.Y = vec.Y;
			if (vec.Z < min.Z) min.Z = vec.Z;
		}

		private void AssignMax(ref Vector3 vec, ref Vector3 max)
		{
			if (vec.X > max.X) max.X = vec.X;
			if (vec.Y > max.Y) max.Y = vec.Y;
			if (vec.Z > max.Z) max.Z = vec.Z;
		}

		private void EnsureValidVector(ref Vector3 vec)
		{
			if (vec.X == float.MinValue || vec.Y == float.MinValue || vec.Z == float.MinValue ||
				vec.X == float.MaxValue || vec.Y == float.MaxValue || vec.Z == float.MaxValue)
			{
				throw new Exception("Unable to create octree for terrain");
			}
		}

		private void PrintTreeDebugInfo(Octree tree)
		{
			var node = tree.Root;
			int treeDepth = 1;

			while (!node.IsLeaf)
			{
				node = node.Children[0];
				treeDepth++;
			}

			Debug.WriteLine("Made Octree, Depth: " + treeDepth + ", Total Nodes: " + _nodeCount);
		}
	}
}
