using System;
using System.IO;
using System.Text;

namespace S3DFileLib
{
	public class S3DFileUtil
	{
		public static byte[] DecodeWldStringBytes(BinaryReader reader, int size)
		{
			return DecodeWldStringBytes(reader.ReadBytes(size));
		}

		public static byte[] DecodeWldStringBytes(byte[] data)
		{
			byte[] encarr = { 0x95, 0x3A, 0xC5, 0x2A, 0x95, 0x7A, 0x95, 0x6A };

			for (int i = 0; i < data.Length; i++)
				data[i] = (byte)(data[i] ^ encarr[i % 8]);

			return data;
		}

		public static string DecodeWldString(BinaryReader reader, int size)
		{
			return DecodeWldString(reader.ReadBytes(size));
		}

		public static string DecodeWldString(byte[] data)
		{
			return Encoding.ASCII.GetString(DecodeWldStringBytes(data));
		}
	}
}
