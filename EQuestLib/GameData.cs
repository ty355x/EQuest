using System;
using EQuestLib.Data;

namespace EQuestLib
{
	public static class GameData
	{
		public static Races Races = new Races();

		static GameData()
		{
			Races.Populate();
		}
	}
}
