using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag2D : Frag
	{
		public Frag2D(BinaryReader reader)
		{
			Ref = reader.ReadInt32();
		}

		public int Ref;
	}
}
