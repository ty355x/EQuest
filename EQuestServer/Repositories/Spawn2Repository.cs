using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EQuestServer.Repositories
{
	public class Spawn2Repository : BaseRepository
	{
		public List<DbSpawn2> FindByZoneName(string zoneName)
		{
			return _context.Spawn2s
				.Where(s => s.zone == zoneName && s.version == 0)
				.Include(s => s.SpawnGroup)
				.Include(s => s.SpawnGroup.SpawnEntries)
				.ToList();
		}
	}
}
