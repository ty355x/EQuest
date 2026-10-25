using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag15 : Frag
	{
		public int Ref0;
		public int Flags;
		public int Ref1;
		public float X;
		public float Y;
		public float Z;
		public float RotateZ;
		public float RotateY;
		public float RotateX;
		public float ScaleY;
		public float ScaleX;

		public Frag15(BinaryReader reader)
		{
			Ref0 = reader.ReadInt32();
			Flags = reader.ReadInt32();
			Ref1 = reader.ReadInt32();
			
			X = reader.ReadSingle();
			Y = reader.ReadSingle();
			Z = reader.ReadSingle();

			RotateZ = reader.ReadSingle();
			RotateY = reader.ReadSingle();
			RotateX = reader.ReadSingle();

			ScaleY = reader.ReadSingle();
			ScaleX = reader.ReadSingle();
		}
	}
}
