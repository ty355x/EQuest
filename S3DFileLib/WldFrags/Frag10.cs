using System.IO;

namespace S3DFileLib.WldFrags
{
	/// <summary>
	/// Skeleton Track Set.
	/// </summary>
	public class Frag10 : Frag
	{
		public uint Flags;
		public int Size;
		public int Frag1Ref;
		public uint Params1_0;
		public uint Params1_1;
		public uint Params1_2;
		public float Params2;
		public Frag10Entry[] Entries;
		public int Size2;
		public int[] Frag3Refs;

		public Frag10(BinaryReader reader)
		{
			Flags = reader.ReadUInt32();
			Size = reader.ReadInt32();
			Frag1Ref = reader.ReadInt32();

			if ((Flags & 0x00000001) == 0x00000001)
			{
				Params1_0 = reader.ReadUInt32();
				Params1_1 = reader.ReadUInt32();
				Params1_2 = reader.ReadUInt32();
			}

			if ((Flags & 0x00000002) == 0x00000002)
				Params2 = reader.ReadSingle();

			Entries = new Frag10Entry[Size];
			for (int i = 0; i < Size; i++)
			{
				Frag10Entry entry = new Frag10Entry();
				Entries[i] = entry;

				entry.NameRef = reader.ReadInt32();
				entry.Flags = reader.ReadUInt32();
				entry.FragRef1 = reader.ReadInt32();
				entry.FragRef2 = reader.ReadInt32();
				entry.Size = reader.ReadInt32();

				entry.Children = new int[entry.Size];
				for (int j = 0; j < entry.Size; j++)
					entry.Children[j] = reader.ReadInt32();
			}

			if ((Flags & 0x00000200) == 0x00000200)
			{
				Size2 = reader.ReadInt32();
				Frag3Refs = new int[Size2];
				for (int i = 0; i < Size2; i++)
					Frag3Refs[i] = reader.ReadInt32();
			}
		}
	}
}
