using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using S3DFileLib.WldFrags;

namespace S3DFileLib
{
	public class WLDFile : IDisposable
	{
		private Frag[] _frags;
		private BinaryReader _reader;
		private byte[] _strings;
		private Dictionary<string, Frag> _fragHash = new Dictionary<string, Frag>();

		public Frag[] Frags { get => _frags; }

		public static WLDFile Open(BinaryReader reader)
		{
			var wld = new WLDFile(reader);
			wld.LoadWldFileData();
			return wld;
		}

		private WLDFile(BinaryReader reader)
		{
			_reader = reader;
		}

		public string GetStringFromRef(int stringRef)
		{
			return GetString(-stringRef);
		}

		public string GetString(int pos)
		{
			if (pos >= _strings.Length)
				return null;

			int endPos = Array.IndexOf<byte>(_strings, 0, pos);

			if (endPos == -1)
			{
				Debug.WriteLine("No terminator found for string at pos " + pos);
				return null;
			}

			return Encoding.ASCII.GetString(_strings, pos, endPos - pos);
		}

		public byte[] GetFragData(string fragName)
		{
			var frag = FindFragByName(fragName);

			if (frag == null)
				return null;

			return GetFragData(frag.FragIndex);
		}

		public byte[] GetFragData(int fragIndex)
		{
			var frag = _frags[fragIndex];
			byte[] data = new byte[frag.FragSize];
			_reader.BaseStream.Position = frag.FragDataPos;
			_reader.Read(data, 0, frag.FragSize);
			return data;
		}

		public BinaryReader GetFragReader(string fragName)
		{
			var frag = FindFragByName(fragName);

			if (frag == null)
				return null;

			return GetFragReader(frag.FragIndex);
		}

		public BinaryReader GetFragReader(int fragIndex)
		{
			_reader.BaseStream.Position = _frags[fragIndex].FragDataPos;
			return _reader;
		}

		public Frag FindFragByName(string name)
		{
			if (_fragHash.ContainsKey(name))
				return _fragHash[name];
			else
				return null;
		}

		public Frag FindFragByRef(int fragRef)
		{
			if (fragRef > 0)
				return _frags[fragRef - 1];

			string fragName = GetString(-(fragRef - 1));

			if (fragName == null)
				return null;

			if (_fragHash.ContainsKey(fragName))
				return _fragHash[fragName];
			else
				return null;
		}

		public Frag FindFragByIndex(int fragIndex)
		{
			foreach (Frag frag in _frags)
			{
				if (frag.FragIndex == fragIndex)
					return frag;
			}

			return null;
		}

		public void Dispose()
		{
			if (_reader != null)
			{
				_reader.Close();
				_reader = null;
			}
		}

		private void LoadWldFileData()
		{
			// Read header magic.
			int magic = _reader.ReadInt32();
			if (magic != 0x54503d02)
				throw new Exception("WLD file header magic value was incorrect");

			// Read version.
			int version = _reader.ReadInt32();
			if (version != 0x00015500)
				throw new Exception("WLD file version " + version + " not supported");

			// Read header data.
			int fragCount = _reader.ReadInt32();
			int header3 = _reader.ReadInt32();
			int header4 = _reader.ReadInt32();
			int stringHashSize = _reader.ReadInt32();
			int header6 = _reader.ReadInt32();

			// Decode the string hash.
			_strings = S3DFileUtil.DecodeWldStringBytes(_reader, stringHashSize);

			// Read the frags.
			ReadFrags(fragCount);

			// Create the frag hash.
			foreach (var frag in Frags)
			{
				if (frag.FragName != null)
					_fragHash[frag.FragName] = frag;
			}
		}

		private void ReadFrags(int fragCount)
		{
			_frags = new Frag[fragCount];
			int currentFragIndex = 0;
			long nextFragPos = _reader.BaseStream.Position;

			for (int i = 0; i < fragCount; i++)
			{
				_reader.BaseStream.Position = nextFragPos;

				int fragIndex = currentFragIndex++;
				int fragSize = _reader.ReadInt32();

				int fragType = _reader.ReadInt32();
				int fragNameRef = -_reader.ReadInt32();
				string fragName = GetString(fragNameRef);
				long fragDataPos = _reader.BaseStream.Position;
				nextFragPos = _reader.BaseStream.Position + fragSize - 4;

				var frag = ReadFragData(fragType);
				_frags[i] = frag;

				frag.FragIndex = fragIndex;
				frag.FragName = fragName;
				frag.FragSize = fragSize;
				frag.FragType = fragType;
				frag.FragDataPos = fragDataPos;
			}
		}

		private Frag ReadFragData(int fragType)
		{
			switch (fragType)
			{
				case 0x03: return new Frag03(_reader);
				case 0x04: return new Frag04(_reader);
				case 0x05: return new Frag05(_reader);
				case 0x10: return new Frag10(_reader);
				case 0x11: return new Frag11(_reader);
				case 0x12: return new Frag12(_reader);
				case 0x13: return new Frag13(_reader);
				case 0x14: return new Frag14(_reader);
				case 0x15: return new Frag15(_reader);
				case 0x21: return new Frag21(_reader);
				case 0x22: return new Frag22(_reader);
				case 0x29: return new Frag29(_reader);
				case 0x2D: return new Frag2D(_reader);
				case 0x2F: return new Frag2F(_reader);
				case 0x30: return new Frag30(_reader);
				case 0x31: return new Frag31(_reader);
				case 0x36: return new Frag36(_reader);
				case 0x37: return new Frag37(_reader);
				default: return new Frag();
			}
		}
	}
}
