using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace EQuestLib
{
	public class PacketIn : IDisposable
	{
		private object _sender;
		private bool _isDisposed = false;
		private byte[] _readBuffer;
		private MemoryStream _readBufferStream;
		protected BinaryReader _reader;
		
		public object Sender { get { return _sender; } }
		
		public PacketIn(object sender, byte[] data, int len)
		{
			_sender = sender;
			_readBuffer = new byte[len];
			Buffer.BlockCopy(data, 0, _readBuffer, 0, len);
			_readBufferStream = new MemoryStream(_readBuffer);
			_reader = new BinaryReader(_readBufferStream);
		}

		public int ReadCommand()
		{
			return ReadInt();
		}

		public float ReadFloat()
		{
			return _reader.ReadSingle();
		}

		public int ReadInt()
		{
			return _reader.ReadInt32();
		}

		public string ReadString()
		{
			return _reader.ReadString();
		}

		public Vector3 ReadVector3()
		{
			return new Vector3(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_reader.Dispose();
				_reader = null;
				_readBufferStream.Dispose();
				_readBufferStream = null;
				_readBuffer = null;
				_isDisposed = true;
			}
		}
	}
}
