using System;
using System.Collections.Generic;
using System.Linq;

namespace EQuestServer.Repositories
{
	public class ZoneRepository : BaseRepository
	{
		public List<DbZone> FindAll()
		{
			return _context.Zones.ToList();
		}
	}
}
