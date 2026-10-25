using System.IO;

namespace S3DFileLib.WldFrags
{
	// BSP Tree entry.
	public class Frag21Entry
	{
		public float NormalX;
		public float NormalY;
		public float NormalZ;
		public float SplitDistance;
		public int RegionID;
		public int Node1;
		public int Node2;
	}
	
	// BSP Tree
	public class Frag21 : Frag
	{
		public uint Size1;
		public Frag21Entry[] Entries;
	
		public Frag21(BinaryReader reader)
		{
			Size1 = reader.ReadUInt32();
			Entries = new Frag21Entry[Size1];

			for (uint i = 0; i < Entries.Length; i++)
			{
				Frag21Entry entry = new Frag21Entry();
				entry.NormalX = reader.ReadSingle();
				entry.NormalY = reader.ReadSingle();
				entry.NormalZ = reader.ReadSingle();
				entry.SplitDistance = reader.ReadSingle();
				entry.RegionID = reader.ReadInt32();
				entry.Node1 = reader.ReadInt32();
				entry.Node2 = reader.ReadInt32();

				Entries[i] = entry;
			}
		}
	}
}
