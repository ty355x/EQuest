using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag13 : Frag
	{
		public int Ref;     // Points to a 0x12.
		public int Flags;
		public int Params1;

		public Frag13(BinaryReader reader)
		{
			Ref = reader.ReadInt32();
			Flags = reader.ReadInt32();
			
			if ((Flags & 1) == 1)
				Params1 = reader.ReadInt32();
		}
	}
}
