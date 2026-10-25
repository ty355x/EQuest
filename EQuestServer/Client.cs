using System;
using System.Net.Sockets;
using EQuestLib;
using EQuestServer.Repositories;
using EQuestServer.Zones.Mobs;

namespace EQuestServer
{
	public class Client : IDisposable
	{
		public int AccountId { get; set; }
		public string Name { get; set; }
		public ClientState State { get; set; }
		public DbCharacter Character { get; set; }
		public string PendingZoneTransferName { get; set; }
		public Mob Mob { get; set; }
		public ISocketHandler SocketHandler { get; set; }
		public bool HasBeenSentInventoryItems { get; set; }

		private Socket _sock;
		private byte[] _receiveBuffer = new byte[1024];
		
		private AsyncCallback _receiveCallback;
		private AsyncCallback _endSendCallback;

		public string ClientIP
		{
			get { return _sock.RemoteEndPoint.ToString(); }
		}

		public Client(string name, Socket sock)
		{
			this.Name = name;
			_sock = sock;
			HasBeenSentInventoryItems = false;
			State = ClientState.PendingLogin;
			_receiveCallback = new AsyncCallback(OnReceive);
			_endSendCallback = new AsyncCallback(OnEndSend);
			BeginReceive();
		}

		public void SaveCharacter()
		{
			Log.Info("Saving character " + Mob.Name);
			
			if (Mob != null)
				Mob.PlayerInventory.SaveInventory();
		}

		public void Send(PacketOut packet)
		{
			packet.Send(_sock, _endSendCallback);
		}

		public void SendMessage(string message)
		{
			PacketOut packet = PacketMaker.MakeMessage(message);
			Send(packet);
		}

		private void OnEndSend(IAsyncResult ar)
		{
			_sock.EndSend(ar);
		}

		private void OnReceive(IAsyncResult ar)
		{
			if (_sock == null)
				return;
			
			int len = 0;

			try
			{
				len = _sock.EndReceive(ar);
			}
			catch (SocketException)
			{
				if (SocketHandler != null)
					SocketHandler.SocketDisconnected(this);

				return;
			}

			if (len > 0 && SocketHandler != null)
			{
				PacketIn packet = new PacketIn(this, _receiveBuffer, len);
				SocketHandler.SocketDataReceived(this, packet);
			}

			BeginReceive();
		}

		public void Dispose()
		{
			if (SocketHandler != null)
				SocketHandler = null;

			if (_sock != null)
			{
				_sock.Close();
				_sock.Dispose();
				_sock = null;
			}
		}

		private void BeginReceive()
		{
			_sock.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, _receiveCallback, null);
		}
	}
}
