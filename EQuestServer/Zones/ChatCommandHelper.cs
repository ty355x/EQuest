using System;

namespace EQuestServer.Zones
{
	static class ChatCommandHelper
	{
		public static string GetAction(string command)
		{
			int spacePos = command.IndexOf(' ');
			return spacePos == -1 ? command : command.Substring(0, spacePos);
		}
		
		public static string GetSingleParam(string command)
		{
			string[] s = command.Split(' ');
			return s.Length >= 2 ? s[1] : null;
		}

		public static string[] GetParams(string command)
		{
			int spacePos = command.IndexOf(' ');
			return command.Substring(spacePos + 1, command.Length - spacePos - 1).Split(' ');
		}
	}
}