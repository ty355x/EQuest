using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestServer.Repositories;
using EQuestServer.Zones.Combat;
using EQuestServer.Zones.Mobs;
using EQuestServer.Zones.Spawns;

namespace EQuestServer.Zones
{
	public class Zone
	{
		public string Name { get; private set; }
		public string LongName { get; private set; }
		public ZoneServer Server { get; private set; }
		public List<Client> Clients { get; private set; }
		public List<Mob> Mobs { get; private set; }
		public ZoneMap Map { get; private set; }
		public CombatHandler Combat { get; private set; }
		public ZoneLines ZoneLines { get; set; }
		public ZoneSpawns Spawns { get; private set; }
		
		private int _currentMobId = 0;
		private long _nextGameTick = -1;
		private const long GameTickInterval = 6000;

		public Zone(ZoneServer server, string name, string longName)
		{
			this.Name = name;
			this.LongName = longName;
			this.Server = server;
			
			Clients = new List<Client>();
			Mobs = new List<Mob>();
			Combat = new CombatHandler(this);
			ZoneLines = new ZoneLines();
		}

		public void Init()
		{
			// Read the zone map.
			var mapReader = new ZoneMapReader();
			Map = mapReader.ReadZoneMap(Name);

			// Load spawns.
			Spawns = new ZoneSpawns(this);
			Spawns.LoadSpawns();

			// Load zone lines.
			ZoneLines.LoadZoneLines(Name);
		}

		public void CheckDoGameTick(long time)
		{
			if (!(_nextGameTick == -1 || time >= _nextGameTick))
				return;

			_nextGameTick = time + GameTickInterval;

			foreach (var mob in Mobs)
				mob.DoRegen();
		}

		public void UpdateMobs(long time)
		{
			foreach (var mob in Mobs)
				mob.Update(time);
		}

		public void UpdateSpawns(long time)
		{
			Spawns.Update(time);
		}

		public void AddClient(Client client)
		{
			Clients.Add(client);
			client.Mob.Id = GetNextMobID();
			client.Mob.Zone = this;

			// Send the client the list of inventory items if necessary.
			if (!client.HasBeenSentInventoryItems)
			{
				var items = client.Mob.PlayerInventory.GetUniqueItems();
				client.Send(PacketMaker.MakeUpdateKnownItems(items));
				client.HasBeenSentInventoryItems = true;
			}
			
			// Send the enter zone packet to the client.
			PacketOut packet = PacketMaker.MakeEnterZone(Name, LongName, client.Mob);
			client.Send(packet);
			Log.Info(string.Format("{0} - Added client {1}, client count {2}", Name, client.Name, Clients.Count));
		}

		public void RemoveClient(Client client)
		{
			// Remove the client from the client list and the mob list.
			Clients.Remove(client);
			Mobs.Remove(client.Mob);

			// Send a remove mob packet to all clients.
			if (client.Mob != null)
			{
				foreach (var mob in Mobs)
					mob.RemoveTargetFromHateList(client.Mob);

				PacketOut packet = PacketMaker.MakeRemoveMob(client.Mob);
				Server.SendPacketToAllActive(packet);
			}

			Log.Info(string.Format("{0} - Removed client {1}, client count {2}", Name, client.Name, Clients.Count));
		}

		public void HandleClientEnteredZone(Client client)
		{
			client.State = ClientState.EnteredZone;

			// Add the client mob to the mob list.
			Server.QueueAddMob(client.Mob);

			// Send the mob list to the client.
			Server.SendMobList(client);
		}

		public void AddMob(Mob mob)
		{
			if (mob.Id == -1)
				mob.Id = GetNextMobID();

			Mobs.Add(mob);
			mob.SetDefaultAI();

			// Send a SpawnMob if the mob is not a corpse. A corpse will have already been sent via
			// the Slain packet.
			if (mob.Type != MobType.Corpse)
			{
				PacketOut packet = PacketMaker.MakeSpawnMob(mob);
				Server.SendPacketToAllActive(packet);
			}
		}

		public void RemoveMob(Mob mob)
		{
			RemoveTargetFromAllHateLists(mob);
			Mobs.Remove(mob);

			if (!mob.IsClient && mob.Spawn != null)
				mob.Spawn.StartRespawnTimer();

			PacketOut packet = PacketMaker.MakeRemoveMob(mob);
			Server.SendPacketToAllActive(packet);
		}

		public void AddCorpseForMob(Mob mob, Mob attacker)
		{
			var corpse = mob.MakeCorpse(GetNextMobID());
			Server.QueueAddMob(corpse);
			Server.SendPacketToAllActive(CombatPacketMaker.MakeSlain(corpse, mob, attacker));
		}

		public Mob FindMob(int id)
		{
			return Mobs.Where(m => m.Id == id).Single();
		}

		public void UpdateClientMovement(Client client, Vector3 position, float moveSpeed, float heading,
			int rotateDirection)
		{
			if (client.Mob == null)
				return;

			// Update the mob.
			client.Mob.Position = position;
			client.Mob.MoveSpeed = moveSpeed;
			client.Mob.Heading = heading;
			client.Mob.Movement.RotateDirection = rotateDirection;

			// Send movement update.
			PacketOut packet = PacketMaker.MakeMobMovementUpdate(client.Mob);
			Server.SendPacketToAllActiveExcept(packet, client);
		}

		public void StartLootCorpse(Client client, int corpseId)
		{
			if (client.State != ClientState.EnteredZone)
				return;

			var corpse = FindMob(corpseId);

			if (corpse == null || corpse.Type != MobType.Corpse)
				return;

			List<DbItem> uniqueItems = corpse.Loot.GetUniqueItems();

			client.Send(PacketMaker.MakeUpdateKnownItems(uniqueItems));
			client.Send(PacketMaker.MakeLootList(corpse));

			corpse.Loot.IsLooted = true;
			corpse.Loot.Cash = 0;
		}

		public void LootCorpseItem(Mob player, int corpseId, int itemIndex)
		{
			var mob = FindMob(corpseId);

			if (mob == null || mob.Type != MobType.Corpse)
				return;

			mob.Loot.LootItem(player, itemIndex);
		}

		public int GetNextMobID()
		{
			return _currentMobId++;
		}

		public void RemoveTargetFromAllHateLists(Mob target)
		{
			foreach (var mob in Mobs)
				mob.RemoveTargetFromHateList(target);
		}
	}
}
