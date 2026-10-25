using System;
using Microsoft.Xna.Framework;

namespace EQuestServer.Zones
{
	public class ChatCommandHandler
	{
		private ZoneServer _server;

		public ChatCommandHandler(ZoneServer server)
		{
			_server = server;
		}
		
		public bool IsChatCommand(string message)
		{
			return message.StartsWith("/") || message.StartsWith("#");
		}
		
		public void HandleChatCommand(Client client, string command)
		{
			string action = ChatCommandHelper.GetAction(command);
			string actionLower = action.ToLower();

			if (actionLower == "#goto")
				HandleGotoCommand(client, command);
			else if (actionLower == "#zone")
				HandleZoneCommand(client, command);
			else
				client.SendMessage("Unrecognized command: " + action);
		}

		private void HandleGotoCommand(Client client, string command)
		{
			string[] pos = ChatCommandHelper.GetParams(command);

			Log.Info(pos[0] + " " + pos[1] + " " + pos[2]);

			if (pos.Length != 3)
			{
				client.SendMessage("Invalid destination format for goto command (x y z)");
				return;
			}

			Vector3? dest = null;

			try
			{
				dest = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
			} 
			catch (Exception)
			{
				client.SendMessage("Invalid destination format for goto command (x,y,z)");
				return;
			}

			if (dest != null)
				_server.MoveClient(client, dest.Value);
		}

		private void HandleZoneCommand(Client client, string command)
		{
			string zoneName = ChatCommandHelper.GetSingleParam(command);

			if (zoneName == null)
			{
				client.SendMessage("Missing zone name parameter");
				return;
			}

			Log.Info("Going to zone " + zoneName);
			_server.HandleAdminZoneRequest(client, zoneName);
		}
	}
}
