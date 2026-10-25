using System.IO;
using S3DFileLib;

namespace S3DFileLib.WldFrags
{
	public class Frag03 : Frag
	{
		public string Name;

		public Frag03(BinaryReader reader)
		{
			reader.BaseStream.Position += 4;
			short nameSize = (short)(reader.ReadInt16() - 1);
			byte[] temp = new byte[nameSize];
			reader.Read(temp, 0, nameSize);
			Name = S3DFileUtil.DecodeWldString(temp);
		}
	}
}
