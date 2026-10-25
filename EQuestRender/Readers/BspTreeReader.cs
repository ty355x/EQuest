using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using S3DFileLib;
using S3DFileLib.WldFrags;
using Debug = System.Diagnostics.Debug;

namespace EQuestRender.Readers
{
	public class BspTreeReader
	{
		public BspTree ReadBspTree(WLDFile wld)
		{
			// Find the Frag21 root frag.
			Frag21 rootFrag = FindRootFrag(wld);

			if (rootFrag == null)
				throw new Exception("BSP tree not found in the specified WLD file");

			// Find BSP nodes.
			BspNode[] nodes = FindBspNodes(rootFrag);
			
			// Build the bsp tree.
			BuildBspTree(nodes[0], nodes);

			// Find BSP regions.
			List<BspRegion> regions = FindBspRegions(wld);

			if (regions.Count == 0)
				throw new Exception("No BSP regions found in the specified WLD file");

			AssignBspRegions(nodes, regions, wld);
			AssignBspRegionFlags(regions, wld);
			
			return new BspTree(nodes[0]);
		}

		private BspNode[] FindBspNodes(Frag21 treeFrag)
		{
			var nodes = new BspNode[treeFrag.Entries.Length];

			for (int i = 0; i < treeFrag.Entries.Length; i++)
				nodes[i] = MakeBspNode(treeFrag.Entries[i]);

			return nodes;
		}

		private BspNode MakeBspNode(Frag21Entry entry)
		{
			var node = new BspNode()
			{
				NormalX = entry.NormalX,
				NormalY = entry.NormalY,
				NormalZ = entry.NormalZ,
				SplitDistance = entry.SplitDistance,
				RegionId = entry.RegionID,
				LeftId = entry.Node1,
				RightId = entry.Node2,
				Plane = new Plane(entry.NormalX, entry.NormalY, entry.NormalZ, entry.SplitDistance)
			};

			return node;
		}

		private Frag21 FindRootFrag(WLDFile wld)
		{
			foreach (var frag in wld.Frags)
			{
				if (frag.FragType == 0x21)
					return frag as Frag21;
			}

			return null;
		}

		private void AssignBspRegions(BspNode[] nodes, List<BspRegion> regions, WLDFile wld)
		{
			foreach (var node in nodes)
			{
				if (node.RegionId == 0)
					continue;

				node.Region = regions[node.RegionId - 1];
			}
		}

		private void AssignRegionFlags(BspNode node, WLDFile wld)
		{
			// Make a list of all the region frag flags.
			var regionFlags = new List<Frag29>();

			foreach (var frag in wld.Frags)
			{
				if (frag.FragType == 0x29)
					regionFlags.Add((Frag29)frag);
			}
		}

		private void BuildBspTree(BspNode node, BspNode[] allNodes)
		{
			// Resolve left nodes.
			if (node.LeftId != 0)
			{
				node.FrontNode = allNodes[node.LeftId - 1];
				BuildBspTree(node.FrontNode, allNodes);
			}

			// Resolve right nodes.
			if (node.RightId != 0)
			{
				node.BackNode = allNodes[node.RightId - 1];
				BuildBspTree(node.BackNode, allNodes);
			}
		}

		private List<BspRegion> FindBspRegions(WLDFile wld)
		{
			// Find the BSP regions.
			var regions = new List<BspRegion>();

			foreach (var frag in wld.Frags)
			{
				if (frag.FragType == 0x22)
					regions.Add(new BspRegion((Frag22) frag, wld));
			}

			return regions;
		}

		private void AssignBspRegionFlags(List<BspRegion> regions, WLDFile wld)
		{
			List<Frag29> flagFrags = new List<Frag29>();

			foreach (var frag in wld.Frags)
			{
				if (frag.FragType == 0x29)
					flagFrags.Add((Frag29)frag);
			}

			foreach (var frag in flagFrags)
			{
				foreach (int regionId in frag.Regions)
					regions[regionId].AssignFlag(frag);
			}
		}

		private void PrintBspTree(BspNode node)
		{	
			Debug.WriteLine("Got bsp plane {0}, {1}, {2}, {3}", node.Plane.Normal.X,
				node.Plane.Normal.Y, node.Plane.Normal.Z, node.Plane.D);

			if (node.Region != null)
				Debug.WriteLine("Node has region " + node.Region.Name);
			else
				Debug.WriteLine("Node has no region");
			
			if (node.FrontNode != null)
				PrintBspTree(node.FrontNode);

			if (node.BackNode != null)
				PrintBspTree(node.BackNode);
		}
	}
}
