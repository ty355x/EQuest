using System;
using System.Collections.Generic;
using System.Linq;

namespace EQuestServer.Repositories
{
	public class ZoneLineRepository : BaseRepository
	{
		public List<DbZoneLine> FindByZoneName(string zoneName)
		{
			return _context.ZoneLines
				.Where(s => s.zone == zoneName)
				.ToList();
		}
	}
}
