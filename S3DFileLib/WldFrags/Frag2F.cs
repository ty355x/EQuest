using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag2F : Frag
	{
		public Frag2F(BinaryReader reader)
		{
			Ref = reader.ReadInt32();
		}

		public int Ref;
	}
}
