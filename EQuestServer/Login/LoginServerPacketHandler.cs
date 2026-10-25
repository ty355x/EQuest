using System;
using EQuestLib;

namespace EQuestServer.Login
{
	public class LoginServerPacketHandler
	{
		private LoginServer _loginServer;
		
		public LoginServerPacketHandler(LoginServer loginServer)
		{
			_loginServer = loginServer;
		}

		public void HandlePacket(PacketIn packet)
		{
			Client client = (Client)packet.Sender;
			ClientCommand command = (ClientCommand)packet.ReadCommand();
			HandleCommand(client, command, packet);
		}

		private void HandleCommand(Client client, ClientCommand command, PacketIn packet)
		{
			if (command == ClientCommand.Login)
				HandleLogin(client, packet);
			else
				Log.Info("Got unknown client command " + command);
		}

		private void HandleLogin(Client client, PacketIn packet)
		{
			string email = packet.ReadString();
			string password = packet.ReadString();
			_loginServer.LoginClient(client, email, password);
		}
	}
}
