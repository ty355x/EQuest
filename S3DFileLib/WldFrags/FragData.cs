namespace S3DFileLib.WldFrags
{
	public struct FragFrame
	{
		public FragVertex[] Verts;
	}

	public struct FragVertex
	{
		public short X;
		public short Y;
		public short Z;
		public int U;
		public int V;
		public short NX;
		public short NY;
		public short NZ;

		public override string ToString()
		{
			return string.Format("X: {0},\tY: {1},\tZ: {2},\tU: {3},\tV: {4},\tNX: {5},\tNY: {6},\tNZ: {7}",
				X, Y, Z, U, V, NX, NY, NZ);
		}

		public string ToTabbedString()
		{
			return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
				X, Y, Z, U, V, NX, NY, NZ);
		}
	}

	public struct FragPoly
	{
		public int Flags;
		public short V0;
		public short V1;
		public short V2;
	}

	public struct FragPolyTex
	{
		public short PolyCount;
		public short TexIndex;
	}
}
