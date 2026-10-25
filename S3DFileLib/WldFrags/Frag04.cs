using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag04 : Frag
	{
		public Frag04(BinaryReader reader)
		{
			Flags = reader.ReadInt32();
			Size = reader.ReadInt32();
			//Params1 = reader.ReadInt32();
			if (Size > 1) Params2 = reader.ReadInt32();

			Refs = new int[Size];
			for (int i = 0; i < Size; i++)
				Refs[i] = reader.ReadInt32();
		}

		public int Flags;
		public int Size;
		public int Params1;
		public int Params2;
		public int[] Refs;
	}
}
