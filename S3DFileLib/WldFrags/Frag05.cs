using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag05 : Frag
	{
		public Frag05(BinaryReader reader)
		{
			Ref = reader.ReadInt32();
		}

		public int Ref;		// 0x04 Frag
	}
}
