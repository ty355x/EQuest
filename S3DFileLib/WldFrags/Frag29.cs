using System.IO;
using System.Diagnostics;
using S3DFileLib;

namespace S3DFileLib.WldFrags
{
	public class Frag29 : Frag
	{
		public int Flags;
		public int Size1;
		public int[] Regions;
		public int Size2;
		public string RegionName;

		public Frag29(BinaryReader reader)
		{
			Flags = reader.ReadInt32();
			Size1 = reader.ReadInt32();

			Regions = new int[Size1];

			for (int i = 0; i < Size1; i++)
				Regions[i] = reader.ReadInt32();

			Size2 = reader.ReadInt32();

			if (Size2 > 0)
				RegionName = S3DFileUtil.DecodeWldString(reader, Size2);
		}
	}
}
