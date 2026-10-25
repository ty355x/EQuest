using System;
using Microsoft.Xna.Framework;

namespace EQuestRender
{
	public class BspNode
	{
		public float NormalX;
		public float NormalY;
		public float NormalZ;
		public float SplitDistance;
		public int RegionId;
		public int LeftId;
		public int RightId;
		public Plane Plane;

		public BspNode FrontNode;
		public BspNode BackNode;

		public BspRegion Region;

		public override string ToString()
		{
			if (Region == null)
				return "No region";
			else
				return string.Format("Normal: {4},{5},{6}, Name: {0}, RegionName: {1}, ZoneId: {2}, ZoneLineId: {3}", 
					Region.Name, Region.RegionName, Region.RequestZoneId, Region.RequestZoneLineId, NormalX, NormalY, NormalZ);
		}
	}
}
