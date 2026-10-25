using System;
using EQuestServer.Login;

namespace EQuestServer
{
	class Program
	{
		public const string AppVersion = "0.2";
		public static LoginServer LoginServer;

		static void Main(string[] args)
		{
			Log.Info("EQuest Server version " + Program.AppVersion);

			// Boot up the login server.
			LoginServer = new LoginServer();
			LoginServer.Start();
		}
	}
}
