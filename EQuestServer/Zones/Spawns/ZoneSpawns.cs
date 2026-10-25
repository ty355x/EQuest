using System;
using System.Collections.Generic;
using EQuestServer.Repositories;

namespace EQuestServer.Zones.Spawns
{
	/// <summary>
	/// Manages the list of spawns for a zone.
	/// </summary>
	public class ZoneSpawns
	{
		private Zone _zone;
		private List<Spawn> _spawns = new List<Spawn>();

		/// <summary>
		/// Creates a ZoneSpawns for the specified zone.
		/// </summary>
		public ZoneSpawns(Zone zone)
		{
			_zone = zone;
		}
		
		/// <summary>
		/// Loads the zone spawn data from the database.
		/// </summary>
		public void LoadSpawns()
		{
			_spawns.Clear();
			List<DbSpawn2> dbSpawn2s = null;
			
			using (var dbSpawn2Repository = new Spawn2Repository())
				dbSpawn2s = dbSpawn2Repository.FindByZoneName(_zone.Name);

			foreach (var dbSpawn2 in dbSpawn2s)
				_spawns.Add(new Spawn(dbSpawn2, _zone));
		}

		/// <summary>
		/// Updates the zone spawns.
		/// </summary>
		public void Update(long time)
		{
			foreach (var spawn in _spawns)
			{
				if (!spawn.IsDisabled && !spawn.IsSpawned && time >= spawn.NextRespawnTime)
					SpawnMobFromSpawn(spawn);
			}
		}

		private void SpawnMobFromSpawn(Spawn spawn)
		{
			var mob = spawn.SpawnMob();

			if (mob == null)
				return;

			_zone.Server.QueueAddMob(mob);
		}
	}
}
