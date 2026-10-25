using System;

namespace EQuestServer
{
	public interface IServer
	{
		void Update(long time);
		void HandleClientDisconnect(Client client);
	}
}
