using System;
using Microsoft.Xna.Framework;

namespace EQuestRender
{
	public class BspTree
	{
		public BspNode Root { get; private set; }
		public BspNode[] Intersections { get; private set; }
		public int IntersectionCount { get; private set; }

		private const int MaxIntersections = 256;

		public BspTree(BspNode root)
		{
			this.Root = root;
			Intersections = new BspNode[MaxIntersections];
			IntersectionCount = 0;
		}

		public void FindIntersectionNodes(BoundingBox box)
		{
			IntersectionCount = 0;
			FindIntersectionNodesRecursive(Root, box);
		}

		private void FindIntersectionNodesRecursive(BspNode node, BoundingBox box)
		{
			var intersect = node.Plane.Intersects(box);

			if (intersect == PlaneIntersectionType.Intersecting)
			{
				Intersections[IntersectionCount++] = node;

				if (IntersectionCount >= MaxIntersections)
					return;

				if (node.FrontNode != null)
					FindIntersectionNodesRecursive(node.FrontNode, box);

				if (node.BackNode != null)
					FindIntersectionNodesRecursive(node.FrontNode, box);
			}
			else if (intersect == PlaneIntersectionType.Front && node.FrontNode != null)
			{
				FindIntersectionNodesRecursive(node.FrontNode, box);
			}
			else if (intersect == PlaneIntersectionType.Back && node.BackNode != null)
			{
				FindIntersectionNodesRecursive(node.BackNode, box);
			}
		}
	}
}
