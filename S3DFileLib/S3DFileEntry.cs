using System.Collections.Generic;
using System.IO;
using zlib;

namespace S3DFileLib
{
	/// <summary>
	/// An file entry in an S3D file.
	/// </summary>
	public class S3DFileEntry
	{
		public S3DFileEntry(uint crc, uint pos, uint size, BinaryReader reader)
		{
			this.Crc = crc;
			this.Pos = pos;
			this.Size = size;
		}

		/// <summary>
		/// Reads the contents of the S3D file entry into a memory stream.
		/// </summary>
		public MemoryStream GetFileStream(BinaryReader reader)
		{
			// Save the current position of the reader.
			long savePos = reader.BaseStream.Position;
						
			try
			{
				// Move the reader to the offset of the current file.
				reader.BaseStream.Position = Pos;
				
				// Create a stream in which to store the result.
				MemoryStream outputStream = new MemoryStream();

				// Read blocks.
				while (outputStream.Length < Size)
				{
					uint defLen = reader.ReadUInt32();
					uint infLen = reader.ReadUInt32();

					// Read the block into a buffer.
					byte[] buffer = reader.ReadBytes((int)defLen);

					// Create temporary streams to unzip the data block.
					MemoryStream unzippedStream = null;
					ZOutputStream zipStream = null;
					
					try
					{
						unzippedStream = new MemoryStream();
						zipStream = new ZOutputStream(unzippedStream);
						zipStream.Write(buffer, 0, buffer.Length);
						zipStream.Flush();
						
						// Write the data block to the data stream.
						unzippedStream.Position = 0;
						unzippedStream.WriteTo(outputStream);
					}
					finally
					{
						if (zipStream != null) zipStream.Close();
						if (unzippedStream != null) unzippedStream.Close();
					}
				}

				outputStream.Position = 0;
				return outputStream;
			}
			finally
			{
				// Restore the base stream to the previous position.
				reader.BaseStream.Position = savePos;
			}
		}

		public override string ToString()
		{
			return "S3D File Entry: " + FileName + "Size: " + Size;
		}

		public string FileName = string.Empty;
		public uint Pos;
		public uint Size;
		public uint Crc;
	}

	public class S3DFileEntryPosComparer : IComparer<S3DFileEntry>
	{
		public int Compare(S3DFileEntry e1, S3DFileEntry e2)
		{
			return e1.Pos.CompareTo(e2.Pos);
		}
	}
}
