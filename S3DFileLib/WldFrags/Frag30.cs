using System.IO;

namespace S3DFileLib.WldFrags
{
	// Texture
	public class Frag30 : Frag
	{
		public Frag30(BinaryReader reader)
		{
			Flags = reader.ReadUInt32();
			Params1 = reader.ReadUInt32();
			Params2 = reader.ReadUInt32();
			Params3_0 = reader.ReadSingle();
			Params3_1 = reader.ReadSingle();
			Ref = reader.ReadInt32();
		}

		public int Ref;		// 0x05 Frag. Can be 0.
		public uint Flags;
		public uint Params1;
		public uint Params2;
		public float Params3_0;
		public float Params3_1;
	}
}
