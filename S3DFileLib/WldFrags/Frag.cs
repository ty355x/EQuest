namespace S3DFileLib.WldFrags
{
	public class Frag
	{
		public const int FragHeaderSize = 16;
		
		public int FragIndex;
		public string FragName;
		public int FragType;
		public int FragSize;
		public long FragDataPos;

		public Frag()
		{
		}

		public Frag(Frag baseFrag)
		{
			FragIndex = baseFrag.FragIndex;
			FragName = baseFrag.FragName;
			FragType = baseFrag.FragType;
			FragSize = baseFrag.FragSize;
			FragDataPos = baseFrag.FragDataPos;
		}

		public override string ToString()
		{
			return string.Format("Frag ({0}) {1}, Type {2}, Size {3}",
				FragIndex, FragName, FragType.ToString("X"), FragSize);
		}
	}
}
