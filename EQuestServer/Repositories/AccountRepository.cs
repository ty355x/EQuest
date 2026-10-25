using System;
using System.Linq;

namespace EQuestServer.Repositories
{
	public class AccountRepository : BaseRepository
	{
		public DbAccount FindByEmail(string email)
		{
			return _context.Accounts.Where(a => a.email == email).FirstOrDefault();
		}

		public void Save(DbAccount account)
		{
			_context.Entry(account).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			_context.SaveChanges();
		}
	}
}
