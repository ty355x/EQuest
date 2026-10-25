using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using EQuestServer.Repositories;
using EQuestServer.Zones;

namespace EQuestServer.Login
{
	public class LoginServer : IServer
	{
		private object UpdateLock = new object();
		private class ZoneListUpdate { public ZoneServer Zone; public UpdateListAction Action; };

		private const int ListenSocket = 1999;
		private const int UpdateIntervalMS = 50;
		
		private bool _isRunning = false;
		private Socket _listenSock = null;

		private ServerThread _serverThread;
		private AsyncCallback _acceptCallback;
		private LoginServerPacketHandler _packetHandler;
		private LoginServerClientList _clients = new LoginServerClientList();
		private AccountLogin _accountLogin;

		private List<ZoneServer> _zones = new List<ZoneServer>();
		private List<ZoneListUpdate> _zoneActionQueue = new List<ZoneListUpdate>();

		public LoginServer()
		{
			GameData.LoadData();
			_serverThread = new ServerThread(this, UpdateIntervalMS, "Login Server");
			_acceptCallback = new AsyncCallback(AcceptConnection);
			_packetHandler = new LoginServerPacketHandler(this);
			_accountLogin = new AccountLogin(this);
		}

		public void Start()
		{
			StopServer();
			_isRunning = true;
			_serverThread.Start();

			if (ServerGameConfig.Instance.BootTestZone != null)
				StartZone(ServerGameConfig.Instance.BootTestZone, ServerGameConfig.Instance.BootTestZone);

			var localEndPoint = new IPEndPoint(IPAddress.Any, ListenSocket);
			_listenSock = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_listenSock.Bind(localEndPoint);
			_listenSock.Listen(10);

			Log.Info("Login server - Listening for connections on " + ListenSocket);
			ResumeAccept();

			while (_isRunning)
			{
				string command = Console.ReadLine();
				HandleCommand(command);
			}
		}

		public void Stop()
		{
			Log.Info("Login Server - Shutting down");
			StopServer();
		}

		public void RemoveServer(ZoneServer server)
		{
			QueueZoneAction(server, UpdateListAction.Remove);
		}

		public void Update(long time)
		{
			lock (UpdateLock)
			{
				ProcessZoneQueue();
				_clients.ProcessClientQueue(_serverThread);

				lock (_serverThread.PacketQueueLock)
				{
					for (int i = 0; i < _serverThread.PacketCount; i++)
					{
						var packet = _serverThread.GetPacket(i);
						_packetHandler.HandlePacket(packet);
					}

					_serverThread.ClearPackets();
				}
			}
		}

		public void HandleClientDisconnect(Client client)
		{
			Log.Info("Login Server - Got disconnect from " + client.Name + " " + client.ClientIP);
			client.State = ClientState.Disconnected;
			_clients.AddClient(client);
		}

		public void LoginClient(Client client, string email, string password)
		{
			_accountLogin.Login(client, email, password);	
		}

		public ZoneServer StartZone(string zoneName, string longName)
		{
			var zone = new ZoneServer(this, zoneName, longName);
			QueueZoneAction(zone, UpdateListAction.Add);
			zone.StartServer();
			return zone;
		}

		public void MoveClientToZone(Client client, DbZoneLine zoneLine)
		{
			var zone = GameData.FindZone(zoneLine.target_zone_id);

			if (zone == null)
			{
				Log.Info("Login Server - Got invalid zone request from " + client + " for zone id" + zoneLine.target_zone_id);
				DisconnectClient(client);
			}

			float x = zoneLine.target_y;
			float y = zoneLine.target_x;
			float z = zoneLine.target_z;
			float heading = client.Mob.Heading;

			if (x == 999999 && client.Mob != null)
				x = client.Mob.Position.X;

			if (y == 999999 && client.Mob != null)
				y = client.Mob.Position.Y;

			if (z == 999999 && client.Mob != null)
				z = client.Mob.Position.Z;

			MoveClientToZone(client, zone.short_name, new Vector3(x, y, z), heading);
		}

		public void MoveClientToZone(Client client, string zoneName, Vector3 pos, float heading)
		{
			var zone = FindZone(zoneName);

			if (zone != null)
			{
				zone.AddNewClient(client, pos, heading);
			}
			else
			{
				var zoneInfo = GameData.FindZone(zoneName);

				if (zoneInfo == null)
				{
					Log.Info("LoginServer - Invalid zone request, zone " + zoneName + " not found");
					DisconnectClient(client);
					return;
				}
				
				var newZone = StartZone(zoneName, zoneInfo.long_name);
				newZone.AddNewClient(client, pos, heading);
			}
		}

		public void DisconnectClient(Client client)
		{
			_clients.RemoveClient(client);
		}

		private ZoneServer FindZone(string zoneName)
		{
			foreach (var zone in _zones)
			{
				if (zone.ZoneName == zoneName)
					return zone;
			}

			return null;
		}

		private void ProcessZoneQueue()
		{
			if (_zoneActionQueue.Count == 0)
				return;
			
			int numZones = _zones.Count;
			
			foreach (var zoneUpdate in _zoneActionQueue)
			{
				if (zoneUpdate.Action == UpdateListAction.Add)
					_zones.Add(zoneUpdate.Zone);
				else
					_zones.Remove(zoneUpdate.Zone);
			}

			_zoneActionQueue.Clear();

			if (_zones.Count != numZones)
				Log.Info("Login Server - Updated zone list, number of zones is " + _zones.Count);
		}

		private void AcceptConnection(IAsyncResult ar)
		{
			if (_listenSock == null)
				return;
			
			Socket clientSock = _listenSock.EndAccept(ar);

			if (clientSock == null)
				return;
			
			var client = new Client("[NewClient]", clientSock);
			Log.Info("Login Server - Got connection from " + client.ClientIP);

			_clients.AddClient(client);			
			ResumeAccept();
		}

		private void ResumeAccept()
		{
			_listenSock.BeginAccept(_acceptCallback, null);
		}

		private void HandleCommand(string command)
		{
			if (command == null)
				return;

			command = command.ToLower();

			if (command == "exit")
				Stop();
			else
				Log.Info("Login Server - Unrecognized command: " + command);
		}

		private void StopServer()
		{
			_isRunning = false;

			if (_listenSock != null)
			{
				_listenSock.Close();
				_listenSock = null;
			}

			_serverThread.Stop(false);
		}

		private void QueueZoneAction(ZoneServer zone, UpdateListAction action)
		{
			_zoneActionQueue.Add(new ZoneListUpdate() { Zone = zone, Action = action });
		}
	}
}
