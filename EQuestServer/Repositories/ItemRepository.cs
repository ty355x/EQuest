using System;
using System.Collections.Generic;
using System.Linq;

namespace EQuestServer.Repositories
{
	public class DBItemRepository : BaseRepository
	{
		private const int ItemIdLimit = 50000;
		
		public List<DbItem> FindAll()
		{
			return _context.Items.Where(i => i.id <= ItemIdLimit).ToList();
		}
	}
}
