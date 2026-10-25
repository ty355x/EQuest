using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag12 : Frag
	{
		public int Flags;
		public int Size;
		public Frag12Frame[] Frames;

		public Frag12(BinaryReader reader)
		{
			Flags = reader.ReadInt32();
			Size = reader.ReadInt32();
			Frames = new Frag12Frame[Size];

			for (int i = 0; i < Size; i++)
				Frames[i] = new Frag12Frame(reader);
		}
	}

	public class Frag12Frame
	{
		public short QW;
		public short QX;
		public short QY;
		public short QZ;
		public short ShiftX;
		public short ShiftY;
		public short ShiftZ;
		public short ShiftDenom;

		public Frag12Frame(BinaryReader reader)
		{
			QW = reader.ReadInt16();
			QX = reader.ReadInt16();
			QY = reader.ReadInt16();
			QZ = reader.ReadInt16();
			ShiftX = reader.ReadInt16();
			ShiftY = reader.ReadInt16();
			ShiftZ = reader.ReadInt16();
			ShiftDenom = reader.ReadInt16();
		}

		public override string ToString()
		{
			return string.Format(
				"QX: {0}, QY: {1}, QZ: {2}, QW: {3}, SX: {4}, SY: {5}, SZ: {6}, SD: {7}",
				QW, QX, QY, QZ, ShiftX, ShiftY, ShiftZ, ShiftDenom);
		}

		public string ToTabbedString()
		{
			float sx = 0, sy = 0, sz = 0;

			if (ShiftDenom != 0)
			{
				sx = ShiftX / (float) ShiftDenom;
				sy = ShiftY / (float) ShiftDenom;
				sz = ShiftZ / (float) ShiftDenom;
			}

			return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", sx, sy, sz, QX, QY, QZ, QW);
		}

		public string ToDetailedString()
		{
			float rotateX = QW != 0 ? (QX / (float) QW) : 0;
			float rotateY = QW != 0 ? (QY / (float) QW) : 0;
			float rotateZ = QW != 0 ? (QZ / (float) QW) : 0;
			float shiftX = QW != 0 ? (ShiftX / (float) ShiftDenom) : 0;
			float shiftY = QW != 0 ? (ShiftY / (float) ShiftDenom) : 0;
			float shiftZ = QW != 0 ? (ShiftZ / (float) ShiftDenom) : 0;
			
			return string.Format(
				"RotateDenom: {0}\n" +
				"RotateXNum: {1}\n" + 
				"RotateYNum: {2}\n" +
				"RotateZNum: {3}\n" +
				"ShiftXNum: {4}\n" +
				"ShiftYNum: {5}\n" +
				"ShiftZNum: {6}\n" +
				"ShiftDenom: {7}\n" +
				"RotateX: {8}\n" +
				"RotateY: {9}\n" +
				"RotateZ: {10}\n" +
				"ShiftX: {11}\n" +
				"ShiftY: {12}\n" +
				"ShiftZ: {13}\n",
				QW, QX, QY, QZ, ShiftX, ShiftY, ShiftZ, ShiftDenom, 
				rotateX, rotateY, rotateZ, shiftX, shiftY, shiftZ);
		}
	}
}
