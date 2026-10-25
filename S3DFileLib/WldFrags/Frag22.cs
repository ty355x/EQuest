using System.IO;
using Debug = System.Diagnostics.Debug;

namespace S3DFileLib.WldFrags
{
	public class Frag22 : Frag
	{
		public uint Flags;
		public int Fragment1;
		public int Size1;
		public int Size2;
		public int Params1;
		public int Size3;
		public int Size4;
		public int Params2;
		public int Size5;
		public int Size6;
		public int Size7;
		public int Fragment2;
		public int Fragment3;
		
		public Frag22(BinaryReader reader)
		{
			Flags = reader.ReadUInt32();
			Fragment1 = reader.ReadInt32();
			Size1 = reader.ReadInt32();
			Size2 = reader.ReadInt32();
			Params1 = reader.ReadInt32();
			Size3 = reader.ReadInt32();
			Size4 = reader.ReadInt32();
			Params2 = reader.ReadInt32();
			Size5 = reader.ReadInt32();
			Size6 = reader.ReadInt32();

			/*
			// Skip Data1. 
			reader.BaseStream.Position += Size1 * 12;

			// Skip Data2.
			reader.BaseStream.Position += Size2 * 8;  

			// Skip Data3.
			for (int i = 0; i < Size3; i++)
			{
				// Read Data3 flag.
				uint data3Flags = reader.ReadUInt32();

				// How many Data3Data1 entries there are.
				int data3Data1Size = reader.ReadInt32();

				// Skip Data3Data1 entries.
				reader.BaseStream.Position += data3Data1Size * 4;

				// Skip 4 Int32s if bit1 of data3Flags is set.
				if ((data3Flags & 2) == 0)
					reader.BaseStream.Position += 16;
			}

			// Skip Data4 entries.
			for (int i = 0; i < Size4; i++)
			{
				uint data4Flags = reader.ReadUInt32();

				// Skip Data4Params1.
				reader.BaseStream.Position += 4;

				uint data4Type = reader.ReadUInt32();

				// Skip Data4Params.
				if (data4Type == 0x8 || data4Type == 0x9 || data4Type == 0xD || data4Type == 0x12 ||
					data4Type == 0xE || data4Type == 0xFFFFFFF1)
				{
					reader.BaseStream.Position += 4;
				} else if (data4Type == 0xA || data4Type == 0xB || data4Type == 0xC)
				{
					reader.BaseStream.Position += 8;
				}

				// Skip Data4 name.
				if ((data4Flags & 4) != 0)
				{
					int data4Size1 = reader.ReadInt32();
					reader.BaseStream.Position += data4Size1;
				}
			}

			// Skip Data5Entries.
			reader.BaseStream.Position += Size5 * 28;

			// Skip Data6 entries.
			for (int i = 0; i < Size6; i++)
			{
				ushort data6size = reader.ReadUInt16();
				
				if ((Flags & 0x20) != 0)
					reader.BaseStream.Position += data6size * 2;
				else if ((Flags & 0x80) != 0)
					reader.BaseStream.Position += data6size;
			}*/
		}

		public string DetermineMeshName()
		{
			return "R" + int.Parse(FragName.Substring(1, FragName.Length - 1)).ToString() + "_DMSPRITEDEF";
		}
	}
}
