using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestServer.Repositories;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones.Spawns
{
	/// <summary>
	/// Represents a zone spawn object which can spawn a mob.
	/// </summary>
	public class Spawn
	{
		public int Id { get; set; }
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float Heading { get; set; }
		public int RespawnTimeSecs { get; set; }
		public long NextRespawnTime { get; set; }
		public bool IsSpawned { get; set; }
		public bool IsDisabled { get; set; }
		public List<SpawnEntry> SpawnEntries { get; private set; }

		private Zone _zone;

		/// <summary>
		/// Creates a Spawn.
		/// </summary>
		public Spawn(DbSpawn2 dbSpawn2, Zone zone)
		{
			_zone = zone;
			SpawnEntries = new List<SpawnEntry>();

			Id = dbSpawn2.id;
			X = dbSpawn2.x;
			Y = dbSpawn2.y;
			Z = dbSpawn2.z;
			Heading = dbSpawn2.heading;
			RespawnTimeSecs = dbSpawn2.respawntime;
			IsSpawned = false;
			IsDisabled = false;
			NextRespawnTime = -1;

			if (dbSpawn2.SpawnGroup != null && dbSpawn2.SpawnGroup.SpawnEntries != null)
			{
				foreach (var dbSpawnEntry in dbSpawn2.SpawnGroup.SpawnEntries)
					SpawnEntries.Add(new SpawnEntry(dbSpawnEntry));
			}
		}

		public void StartRespawnTimer()
		{
			NextRespawnTime = _zone.Server.CurrentTime + RespawnTimeSecs * 1000;
			IsSpawned = false;
		}
		
		/// <summary>
		/// Creates a mob from the list of spawn entries. Will also update the IsSpawned status of the spawn.
		/// </summary>
		public Mob SpawnMob()
		{
			IsSpawned = false;
			
			if (SpawnEntries.Count == 0)
			{
				IsDisabled = true;
				return null;
			}

			var npcType = GetDbNpcType(SpawnEntries[0].NpcId);

			if (npcType == null)
			{
				Log.Error(_zone.Name + " - Unable to spawn mob for spawn " + Id);
				IsDisabled = true;
				return null;
			}

			IsSpawned = true;			
			return MakeMobFromDbNpcType(npcType);
		}

		private Mob MakeMobFromDbNpcType(DbNpcType npcType)
		{
			var mob = new Mob(_zone);
			mob.Spawn = this;
			mob.Name = npcType.name.Replace("#", "").Replace('_', ' ');
			mob.Type = MobType.Monster;
			mob.TextureIndex = npcType.texture;
			mob.Size = npcType.size;
			mob.Spawn = this;
			mob.Race = npcType.race;
			mob.Gender = npcType.gender;
			mob.Movement.SetPosition(new Vector3(Y, X, Z));
			mob.Heading = Heading;

			mob.MinDamage = npcType.mindmg;
			mob.MaxDamage = npcType.maxdmg;
			mob.HitPoints = npcType.hp;
			mob.MaxHitPoints = npcType.hp;
			
			mob.IsAggro = false;
			mob.AggroRadius = 10;
			mob.CanHitRadius = 10;

			if (npcType.LootTable != null)
				mob.Loot.AddLoot(npcType.LootTable);

			return mob;
		}

		private DbNpcType GetDbNpcType(int id)
		{
			using (var npcRepository = new NpcTypeRepository())
				return npcRepository.Find(id);
		}
	}
}
