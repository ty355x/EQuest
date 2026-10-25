using System;
using Microsoft.Xna.Framework;
using EQuestRender;

namespace EQuestLib.Octrees
{
	public class OctreeCollision
	{
		public OctreeNode[] Nodes { get; private set; }
		public int NodeCount { get; private set; }
		private Octree _octree;

		public OctreeCollision(Octree octree, int maxCollisionNodes)
		{
			_octree = octree;
			Nodes = new OctreeNode[maxCollisionNodes];
			NodeCount = 0;
		}

		public bool BoxCollidesWithObjectInTree(BoundingBox box)
		{
			Reset();
			FindNodesBoxCollidesWith(box);

			for (int i = 0; i < NodeCount; i++)
			{
				if (Nodes[i].BoxCollidesWithObject(box))
					return true;
			}

			return false;
		}

		public PolyCollisionResult FindPolyBoxCollidesWith(BoundingBox box)
		{
			Reset();
			FindNodesBoxCollidesWith(box);

			for (int i = 0; i < NodeCount; i++)
			{
				PolyCollisionResult result = Nodes[i].FindPolyBoxCollidesWith(box);

				if (result.HasCollided)
					return result;
			}

			return PolyCollisionResult.MakeNoCollisionResult();
		}

		public void Reset()
		{
			for (int i = 0; i < Nodes.Length; i++)
			{
				if (Nodes[i] == null)
					break;

				Nodes[i] = null;
			}

			NodeCount = 0;
		}

		public void FindNodesBoxCollidesWith(BoundingBox box)
		{
			FindNodesBoxCollidesWithInternal(_octree.Root, box);
		}

		private void FindNodesBoxCollidesWithInternal(OctreeNode node, BoundingBox box)
		{
			if (NodeCount >= Nodes.Length)
			{
				return;
			}

			if (!node.Bounds.Intersects(box))
			{
				return;
			}

			if (node.IsLeaf)
			{
				Nodes[NodeCount++] = node;
				return;
			}

			foreach (var child in node.Children)
			{
				FindNodesBoxCollidesWithInternal(child, box);
			}
		}
	}
}
