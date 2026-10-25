using System;
using EQuestServer.Repositories;

namespace EQuestServer.Zones.Spawns
{
	public class SpawnEntry
	{
		public int NpcId;
		public int Chance;

		public SpawnEntry(DbSpawnEntry dbSpawnEntry)
		{
			NpcId = dbSpawnEntry.npcID;
			Chance = dbSpawnEntry.chance;
		}
	}
}
