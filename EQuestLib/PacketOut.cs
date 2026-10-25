using System;
using System.IO;
using System.Net.Sockets;

using Microsoft.Xna.Framework;

namespace EQuestLib
{
	public class PacketOut
	{
		private byte[] _buffer = new byte[1024];
		private MemoryStream _outStream;
		private BinaryWriter _writer;
		private long _length = 0;

		public static PacketOut MakePacket(ServerCommand command)
		{
			return new PacketOut((int)command);
		}

		public static PacketOut MakePacket(ClientCommand command)
		{
			return new PacketOut((int)command);
		}

		public PacketOut(int command)
		{
			_outStream = new MemoryStream(_buffer);
			_writer = new BinaryWriter(_outStream);
			WriteInt(command);
		}

		public PacketOut Send(Socket socket, AsyncCallback endSendCallback)
		{
			SocketError errorCode;
			socket.BeginSend(_buffer, 0, (int)_length, SocketFlags.None, out errorCode, endSendCallback, null);
			return this;
		}

		public PacketOut WriteBool(bool value)
		{
			_writer.Write(value);
			return this;
		}

		public PacketOut WriteFloat(float value)
		{
			_writer.Write(value);
			return this;
		}

		public PacketOut WriteInt(int value)
		{
			_writer.Write(value);
			return this;
		}

		public PacketOut WriteString(string value)
		{
			if (value == null)
				_writer.Write(string.Empty);
			else
				_writer.Write(value);
			
			return this;
		}

		public PacketOut WriteVector3(Vector3 value)
		{
			_writer.Write(value.X);
			_writer.Write(value.Y);
			_writer.Write(value.Z);
			return this;
		}

		private void Reset()
		{
			_outStream.Position = 0;
			_outStream.SetLength(0);
		}

		public PacketOut End()
		{
			if (_outStream != null)
			{
				_length = _outStream.Length;
				_outStream.Dispose();
				_outStream = null;
			}

			if (_writer != null)
			{
				_writer.Dispose();
				_writer = null;
			}

			return this;
		}
	}
}
