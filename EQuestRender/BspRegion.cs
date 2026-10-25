using System;
using S3DFileLib;
using S3DFileLib.WldFrags;
using Debug = System.Diagnostics.Debug;

namespace EQuestRender
{
	public enum BspRegionType
	{
		Normal,
		Water,
		Lava,
		PVP,
		ZoneLine
	}
	
	public class BspRegion
	{
		public string Name;
		public BspRegionType Type;
		public string RegionName;
		public uint Flags;
		public string MeshName;
		public ZoneTerrain Terrain;
		
		public int RequestZoneId;
		public int RequestZoneLineId;

		public BspRegion(Frag22 frag, WLDFile wld)
		{
			Name = frag.FragName;
			Type = BspRegionType.Normal;
			MeshName = frag.DetermineMeshName();
			Flags = frag.Flags;
		}

		public void AssignFlag(Frag29 frag)
		{
			if (frag.RegionName != null)
				RegionName = frag.RegionName;
			
			if (frag.RegionName != null)
				ParseRegionName(frag.RegionName);
			else if (frag.FragName != null)
				ParseFragName(frag.FragName);
		}

		private void ParseRegionName(string name)
		{
			if (name.StartsWith("WTN__"))
				Type = BspRegionType.Water;
			else if (name.StartsWith("DRNTP"))
				ParseZoneRequest(name);
		}

		private void ParseFragName(string name)
		{
			if (name == "WT_ZONE")
				Type = BspRegionType.Water;
			else if (name == "LA_ZONE")
				Type = BspRegionType.Lava;
			else if (name == "DRP_ZONE")
				Type = BspRegionType.PVP;
			else if (name.StartsWith("DRNTP"))
				ParseZoneRequest(name);
		}

		private void ParseZoneRequest(string name)
		{
			int zoneId;
			
			if (!int.TryParse(name.Substring(7, 3), out zoneId))
			{
				RenderLog.Debug("Got invalid zone request " + name);
				return;
			}

			if (zoneId == 0)
			{
				Debug.WriteLine("Got invalid zone request id (0)");
				return;
			}
			else if (zoneId != 255)
			{
				RequestZoneId = zoneId;
				Type = BspRegionType.ZoneLine;
			}
			else
			{
				RequestZoneLineId = int.Parse(name.Substring(13, 3));
				Type = BspRegionType.ZoneLine;
			}
		}
	}
}
