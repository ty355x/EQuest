using System;

namespace S3DFileLib.WldFrags
{
	public class Frag10Entry
	{
		public int NameRef;
		public uint Flags;
		public int FragRef1;        // Ref to the 0x13 for this piece.
		public int FragRef2;
		public int Size;
		public int[] Children;      // Next 0x13 pieces.
	}
}
