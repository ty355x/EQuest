using System;
using System.Linq;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Repositories
{
	public class CharacterRepository : BaseRepository
	{
		public DbCharacter FindForAccount(int accountId)
		{
			return _context.Characters.Where(c => c.account_id == accountId).FirstOrDefault();
		}

		public void Save(Mob mob)
		{
		}
	}
}
