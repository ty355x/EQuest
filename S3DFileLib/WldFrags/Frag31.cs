using System.IO;

namespace S3DFileLib.WldFrags
{
	// Texture List
	public class Frag31 : Frag
	{
		public Frag31(BinaryReader reader)
		{
			Flags = reader.ReadInt32();
			Size = reader.ReadInt32();
			Refs = new int[Size];

			for (int i = 0; i < Size; i++)
				Refs[i] = reader.ReadInt32();
		}

		public int Flags;
		public int Size;
		public int[] Refs;	// List of 0x30 Texture Frags
	}
}
