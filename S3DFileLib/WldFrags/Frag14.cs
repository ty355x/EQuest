using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag14 : Frag
	{
		public int Size1;
		public int Size2;
		public short Short1;
		public short Short2;
		public short Short3;
		public short Short4;
		public short Short5;
		public short Short6;
		public short Short7;
		public short Short8;

		public int[] Ref;

		public Frag14(BinaryReader reader)
		{
			reader.BaseStream.Position += 8;
			Size1 = reader.ReadInt32();
			Size2 = reader.ReadInt32();

			// reader.BaseStream.Position += 16;
			Short1 = reader.ReadInt16();
			Short2 = reader.ReadInt16();
			Short3 = reader.ReadInt16();
			Short4 = reader.ReadInt16();
			Short5 = reader.ReadInt16();
			Short6 = reader.ReadInt16();
			Short7 = reader.ReadInt16();
			Short8 = reader.ReadInt16();

			Ref = new int[Size2];
			
			for (int i = 0; i < Size2; i++)
				Ref[i] = reader.ReadInt32();
		}
	}
}
