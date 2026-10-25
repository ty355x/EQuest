using System;

namespace EQuestServer
{
	public enum ClientState
	{
		PendingLogin,
		EnteringZone,
		PendingEnterZone,
		EnteredZone,
		Disconnected,
	}
}
