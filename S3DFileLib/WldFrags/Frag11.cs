using System.IO;

namespace S3DFileLib.WldFrags
{
	/// <summary>
	/// Skeleton Track Set Reference
	/// </summary>
	public class Frag11 : Frag
	{
		public Frag11(BinaryReader reader)
		{
			Ref = reader.ReadInt32();
		}

		public int Ref;		// Ref to a 0x10.
	}
}
