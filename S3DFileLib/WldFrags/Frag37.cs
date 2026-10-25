using System;
using System.IO;

namespace S3DFileLib.WldFrags
{
	public class Frag37 : Frag
	{
		public int Flags;
		public short VertCount;
		public short FrameCount;
		public short Param1;
		public short Param2;
		public short Scale;
		public FragFrame[] Frames;

		public Frag37(BinaryReader reader)
		{
			Flags = reader.ReadInt32();
			VertCount = reader.ReadInt16();
			FrameCount = reader.ReadInt16();
			Param1 = reader.ReadInt16();
			Param2 = reader.ReadInt16();
			Scale = reader.ReadInt16();

			Frames = new FragFrame[FrameCount];
			
			for (int i = 0; i < FrameCount; i++)
			{
				Frames[i].Verts = new FragVertex[VertCount];

				for (int j = 0; j < VertCount; j++)
				{
					FragVertex vert = new FragVertex();
					vert.X = reader.ReadInt16();
					vert.Y = reader.ReadInt16();
					vert.Z = reader.ReadInt16();
					Frames[i].Verts[j] = vert;
				}
			}
		}
	}
}
