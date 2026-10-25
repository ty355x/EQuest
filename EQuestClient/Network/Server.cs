using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using EQuestLib;

namespace EQuestClient.Network
{
	public class Server
	{
		public bool IsConnected { get { return _connected; } }
		public bool HasPackets { get { return _packetBuffer.Count > 0; } }
		public object Lock { get; }
		
		private Socket _sock;
		private byte[] _receiveBuffer = new byte[1024];
		private bool _connected = false;
		private AsyncCallback _receiveCallback;
		private AsyncCallback _endSendCallback;

		private List<PacketIn> _packetBuffer = new List<PacketIn>();

		public Server()
		{
			Lock = new object();
			_receiveCallback = new AsyncCallback(ReceiveData);
			_endSendCallback = new AsyncCallback(EndSendCallback);
		}

		public void Start(string address, int port)
		{
			Close();

			IPHostEntry hostEntry = Dns.GetHostEntry(address);
			IPAddress serverAddress = hostEntry.AddressList[0];
			
			_sock = new Socket(serverAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_sock.Connect(serverAddress, port);
			
			_connected = true;
			StartReceive();
		}

		public void Close()
		{
			if (_sock != null)
			{
				_sock.Close();
				_sock = null;
			}
		}

		public void Send(PacketOut packet)
		{
			if (!_connected)
				return;

			packet.Send(_sock, _endSendCallback);
		}

		public void StartReceive()
		{
			if (!_connected)
				return;

			SocketError errorCode;
			_sock.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, out errorCode, _receiveCallback, null);
		}

		public List<PacketIn> GetPackets()
		{
			return _packetBuffer;
		}

		private void ReceiveData(IAsyncResult ar)
		{
			int size = 0;

			try
			{
				size = _sock.EndReceive(ar);
			}
			catch (SocketException)
			{
				_connected = false;
				throw new Exception("Lost connection to server");
			}

			if (size > 0)
			{
				lock (Lock)
					_packetBuffer.Add(new PacketIn(_sock, _receiveBuffer, size));
			}

			StartReceive();
		}

		private void EndSendCallback(IAsyncResult ar)
		{
			_sock.EndSend(ar);
		}
	}
}
