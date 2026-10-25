using System;

namespace EQuestServer.Repositories
{
	public class BaseRepository : IDisposable
	{
		protected EQuestContext _context = null;
		
		public BaseRepository()
		{
			_context = new EQuestContext();
		}

		public void Dispose()
		{
			if (_context != null)
			{
				_context.Dispose();
				_context = null;
			}
		}
	}
}
