using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Trace = System.Diagnostics.Trace;

namespace S3DFileLib
{
	public class S3DFile : IDisposable
	{
		private List<S3DFileEntry> _entries = new List<S3DFileEntry>();
		private BinaryReader _reader = null;
		public List<S3DFileEntry> Entries { get { return _entries; } }

		public static S3DFile Open(string path)
		{
			var s3d = new S3DFile();
			s3d.LoadS3DFile(path);
			return s3d;
		}
		
		private S3DFile()
		{
		}

		public byte[] GetFileBytes(string fileName)
		{
			S3DFileEntry entry = FindEntry(fileName);

			if (entry == null)
				return null;

			using (MemoryStream dataStream = entry.GetFileStream(_reader))
			{
				byte[] data = new byte[(int)dataStream.Length];
				dataStream.Read(data, 0, data.Length);
				return data;
			}
		}

		public MemoryStream GetFileStream(string fileName)
		{
			S3DFileEntry entry = FindEntry(fileName);

			if (entry == null)
				return null;

			return entry.GetFileStream(_reader);
		}

		public WLDFile GetWldFile(string fileName)
		{
			return WLDFile.Open((new BinaryReader(GetFileStream(fileName))));
		}

		public void Dispose()
		{
			if (_reader != null)
			{
				_reader.Close();
				_reader = null;
			}
		}
		
		private S3DFileEntry FindEntry(string fileName)
		{
			foreach (S3DFileEntry entry in _entries)
			{
				if (string.Compare(entry.FileName, fileName, true) == 0)
					return entry;
			}

			return null;
		}

		private void LoadS3DFile(string path)
		{
			Trace.WriteLine("Opening S3D file " + path + ".");
			_reader = new BinaryReader(File.OpenRead(path));
			
			// Read the directory offset and magic cookie chars.
			uint dirOffset = _reader.ReadUInt32();
			string cookie = new string(_reader.ReadChars(4));

			// Check that the magic cookie is what we expect.
			if (cookie != "PFS ")
				throw new Exception("Invalid S3D file: magic cookie value was invalid.");

			// Move to the dir offset and read the number of entries.
			_reader.BaseStream.Position = dirOffset;
			uint entriesCount = _reader.ReadUInt32();

			if (entriesCount < 1)
				throw new Exception("Invalid S3D File: Entries count is less than 1.");

			// Trace.WriteLine("Found " + entriesCount + " S3D File entries.");
			string[] fileNames = null;

			// Load entries.
			for (int i = 0; i < entriesCount; i++)
			{				
				S3DFileEntry entry = new S3DFileEntry(
					_reader.ReadUInt32(), _reader.ReadUInt32(), _reader.ReadUInt32(), _reader);

				if (entry.Crc == 0x61580AC9)
				{
					// The current entry is the list of file names.
					fileNames = ReadFileNames(entry);
				}
				else
				{
					_entries.Add(entry);
				}
			}

			if (fileNames == null)
				throw new Exception("Unable to load file names from S3D file.");

			if (_entries.Count != entriesCount - 1)
				throw new Exception("File names count does not match files count in S3D file.");

			// Sort entries by offset.
			_entries.Sort(new S3DFileEntryPosComparer());

			// Assign file names.
			for (int i = 0; i < _entries.Count; i++)
				_entries[i].FileName = fileNames[i];
		}

		private string[] ReadFileNames(S3DFileEntry entry)
		{
			byte[] fnData = null;

			using (MemoryStream dataStream = entry.GetFileStream(_reader))
			{
				fnData = new byte[dataStream.Length];
				dataStream.Read(fnData, 0, (int)dataStream.Length);
			}

			int fnCount = (int) BitConverter.ToUInt32(fnData, 0);
			string[] fileNames = new string[fnCount];
			int currentIndex = 4;

			for (int i = 0; i < fnCount; i++)
			{
				int fileNameSize = (int) BitConverter.ToUInt32(fnData, currentIndex);
				fileNames[i] = Encoding.ASCII.GetString(fnData, currentIndex + 4, fileNameSize - 1);
				currentIndex += fileNameSize + 4;
			}

			return fileNames;
		}
	}
}
