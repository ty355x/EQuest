using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestServer.Login;
using EQuestServer.Repositories;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones
{
	public class ZoneServer : IServer
	{
		public static Random Random = new Random();

		private const int UpdateIntervalMS = 50;
		private const int ShutdownDelaySecs = 60;

		public string ZoneName { get; private set; }
		public Zone Zone { get; private set; }
		public ZoneServerState State { get; private set; }
		public ChatCommandHandler ChatCommandHandler { get; private set; }		
				
		public long StartupTime { get; set; }
		public long CurrentTime { get; set; }

		private List<Client> _clientAddQueue = new List<Client>();
		private List<Client> _clientRemoveQueue = new List<Client>();
				
		private List<Mob> _mobAddQueue = new List<Mob>();
		private List<Mob> _mobRemoveQueue = new List<Mob>();

		private LoginServer _loginServer;
		private ServerThread _serverThread;
		private ZonePacketHandler _packetHandler;
		private object _updateLock = new object();
		private bool _requiresBoot = true;
		private bool _isShuttingDown = false;
		private long _shutdownTime = -1;

		public ZoneServer(LoginServer loginServer, string zoneName, string longName)
		{
			_loginServer = loginServer;
			this.ZoneName = zoneName;
			
			StartupTime = -1;
			CurrentTime = 0;
			Zone = new Zone(this, ZoneName, longName);
			State = ZoneServerState.Down;
			ChatCommandHandler = new ChatCommandHandler(this);

			_packetHandler = new ZonePacketHandler(this);
			_serverThread = new ServerThread(this, UpdateIntervalMS, ZoneName);
		}

		public void StartServer()
		{
			if (State != ZoneServerState.Down)
				throw new Exception(ZoneName + " - Unable to boot zone: zone server is not down");

			_requiresBoot = true;
			_serverThread.Start();
		}

		public void StopServer(bool blockUntilTerminated)
		{
			Log.Info(Zone.Name + " - Shutting down");

			State = ZoneServerState.Down;
			_loginServer.RemoveServer(this);
			_serverThread.Stop(blockUntilTerminated);
			
			Log.Info(Zone.Name + " - Shut down");
		}

		public void Update(long time)
		{
			lock (_updateLock)
			{
				CurrentTime = time;

				if (_requiresBoot)
				{
					BootServer();
					return;
				}

				if (State != ZoneServerState.Active)
					return;

				// process packets.
				ProcessPacketQueue();

				// Update mobs.
				Zone.UpdateMobs(time);

				// Process the mob queue.
				ProcessRemoveMobQueue();
				ProcessAddMobQueue();

				// Process the client queue.
				ProcessRemoveClientQueue();
				ProcessAddClientQueue();

				// Check if we need to shut down.
				if (_isShuttingDown && time >= _shutdownTime)
				{
					StopServer(false);
					return;
				}

				// Run a game tick if necessary.
				Zone.CheckDoGameTick(time);

				// Send mob stat updates.
				foreach (var mob in Zone.Mobs)
				{
					if (mob.StatsChanged)
					{
						PacketOut packet = PacketMaker.MakeUpdateMobStats(mob);
						SendPacketToAllActive(packet);
						mob.StatsChanged = false;
					}
				}

				// Update the zone.
				Zone.UpdateSpawns(time);
			}
		}

		public void QueueAddMob(Mob mob)
		{
			lock (_mobAddQueue)
				_mobAddQueue.Add(mob);
		}

		public void QueueRemoveMob(Mob mob)
		{
			lock (_mobRemoveQueue)
				_mobRemoveQueue.Add(mob);
		}

		public void HandleClientZoneRequest(Client client, int zoneId, Vector3 clientPos)
		{
			var zoneLine = Zone.ZoneLines.FindZoneLineForZone(zoneId);

			if (zoneLine == null)
			{
				Log.Info(ZoneName + " - Got invalid zone request for zone " + zoneId + " from " + client.Name);
				return;
			}

			SendClientToZone(client, clientPos, zoneLine);
		}

		public void HandleClientZoneLineRequest(Client client, int zoneLineId, Vector3 clientPos)
		{
			var zoneLine = Zone.ZoneLines.FindZoneLineForZoneLineId(zoneLineId);
			
			if (zoneLine == null)
			{
				Log.Info(ZoneName + " - Got invalid zone request for zone line " + zoneLineId + " from " + client.Name);
				return;
			}

			SendClientToZone(client, clientPos, zoneLine);
		}

		public void HandleClientDisconnect(Client client)
		{
			Log.Info(ZoneName + " - Client " + client.Name + " disconnected");
			QueueRemoveClient(client);
		}

		public void AddNewClient(Client client, Vector3 pos, float heading)
		{
			client.Mob.Position = pos;
			client.Mob.Heading = heading;
			client.Mob.SpawnPoint = new Vector3(0, 0, 0);

			client.SocketHandler = new SocketHandler(_serverThread);
			client.State = ClientState.EnteringZone;
			client.PendingZoneTransferName = ZoneName;

			client.SaveCharacter();
			QueueAddClient(client);
		}

		public void SendMessageToAll(string message)
		{
			PacketOut packet = PacketMaker.MakeMessage(message);
			SendPacketToAllActive(packet);
		}

		public void SendClientMessageToAll(Client client, string message)
		{
			string text = client.Name + ": " + message;
			PacketOut packet = PacketMaker.MakeMessage(text);
			SendPacketToAllActive(packet);
		}

		public void SendMobList(Client client)
		{
			foreach (var mob in Zone.Mobs)
			{
				if (mob != client.Mob)
					client.Send(PacketMaker.MakeSpawnMob(mob));
			}
		}

		public void SendMobMovementUpdate(Mob mob)
		{
			PacketOut packet = PacketMaker.MakeMobMovementUpdate(mob);
			SendPacketToAllActive(packet);
		}

		public void SendMessageToClient(Client client, string message)
		{
			PacketOut packet = PacketMaker.MakeMessage(message);
			client.Send(packet);
		}

		public void SendPacketToAll(PacketOut packet)
		{
			SendPacketToAllExcept(packet, null);
		}

		public void SendPacketToAllExcept(PacketOut packet, Client except)
		{
			foreach (var client in Zone.Clients)
			{
				if (client != except)
					client.Send(packet);
			}
		}

		public void SendPacketToAllActive(PacketOut packet)
		{
			foreach (var client in Zone.Clients)
			{
				if (client.State == ClientState.EnteredZone)
					client.Send(packet);
			}
		}

		public void SendPacketToAllActiveExcept(PacketOut packet, Client except)
		{
			foreach (var client in Zone.Clients)
			{
				if (client.State == ClientState.EnteredZone && client != except)
					client.Send(packet);
			}
		}

		public void HandleAdminZoneRequest(Client client, string zoneName)
		{
			SendClientToZoneSafeCoords(client, zoneName);
		}

		public void MoveClient(Client client, Vector3 dest)
		{
			if (client.State == ClientState.EnteredZone && client.Mob != null)
			{
				client.Mob.Position = dest;
				var packet = PacketMaker.MakeMobMovementUpdate(client.Mob);
				SendPacketToAll(packet);
			}
		}

		private void BootServer()
		{
			if (State != ZoneServerState.Down)
				throw new Exception(ZoneName + " - Unable to boot zone: zone server state is not down");

			Log.Info(ZoneName + " - Zone server is booting");

			_requiresBoot = false;
			State = ZoneServerState.Booting;
			StartupTime = CurrentTime;
			
			Zone.Init();
			State = ZoneServerState.Active;

			Log.Info(ZoneName + " - Zone server is active");
		}

		private void ProcessPacketQueue()
		{
			lock (_serverThread.PacketQueueLock)
			{
				for (int i = 0; i < _serverThread.PacketCount; i++)
					_packetHandler.HandlePacket(_serverThread.GetPacket(i));

				_serverThread.ClearPackets();
			}
		}

		private void ProcessAddMobQueue()
		{
			lock (_mobAddQueue)
			{
				foreach (var mob in _mobAddQueue)
					Zone.AddMob(mob);
				
				_mobAddQueue.Clear();
			}
		}

		private void ProcessRemoveMobQueue()
		{
			lock (_mobRemoveQueue)
			{
				foreach (var mob in _mobRemoveQueue)
					Zone.RemoveMob(mob);
					
				_mobRemoveQueue.Clear();
			}
		}

		private void ProcessAddClientQueue()
		{
			lock (_clientAddQueue)
			{
				foreach (var client in _clientAddQueue)
					Zone.AddClient(client);
				
				_clientAddQueue.Clear();
			}

			if (_isShuttingDown && Zone.Clients.Count > 0)
				_isShuttingDown = false;
		}

		private void ProcessRemoveClientQueue()
		{
			lock (_clientRemoveQueue)
			{
				foreach (var client in _clientRemoveQueue)
					Zone.RemoveClient(client);

				_clientRemoveQueue.Clear();
			}

			if (!_isShuttingDown && Zone.Clients.Count == 0)
			{
				_isShuttingDown = true;
				_shutdownTime = CurrentTime + ShutdownDelaySecs * 1000;
			}
		}

		private void QueueAddClient(Client client)
		{
			lock (_clientAddQueue)
				_clientAddQueue.Add(client);
		}

		private void QueueRemoveClient(Client client)
		{
			lock (_clientRemoveQueue)
				_clientRemoveQueue.Add(client);
		}

		private void SendClientToZone(Client client, Vector3 clientPos, DbZoneLine zoneLine)
		{
			client.State = ClientState.EnteringZone;
			client.Mob.Position = clientPos;
			QueueRemoveClient(client);
			_loginServer.MoveClientToZone(client, zoneLine);
		}

		private void SendClientToZoneSafeCoords(Client client, string zoneName)
		{
			Log.Info("Searching for zone " + zoneName);
			var zone = GameData.FindZone(zoneName.ToLower());

			if (zone == null)
			{
				client.SendMessage("Zone " + zoneName + " not found");
				return;
			}

			client.State = ClientState.EnteringZone;
			QueueRemoveClient(client);
			client.Mob.Position = new Vector3(zone.safe_y, zone.safe_x, zone.safe_z);
			client.Mob.Heading = 0;
			_loginServer.MoveClientToZone(client, zoneName, client.Mob.Position, client.Mob.Heading);
		}
	}
}
