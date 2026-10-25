using System;
using System.Collections.Generic;

namespace EQuestLib.Data
{
	public class Races
	{
		public const string DefaultModelName = "HUM";
		public List<RaceInfo> Entries = new List<RaceInfo>();

		public ModelInfo FindModelInfo(int race, int gender)
		{
			return FindModelName(race, (Gender)gender);
		}
		
		public ModelInfo FindModelName(int race, Gender gender)
		{
			var entry = FindEntry(race);
			string modelName = null;

			if (entry == null)
			{
				return new ModelInfo(DefaultModelName, 1);
			}

			if (gender == Gender.Male)
				modelName = entry.MaleModel;
			else if (gender == Gender.Female)
				modelName = entry.FemaleModel;
			else
				modelName = entry.NeuterModel;

			return new ModelInfo(modelName, entry.Size);
		}

		public RaceInfo FindEntry(int race)
		{
			if (race > 0 && race < Entries.Count)
				return Entries[race];
			else
				return null;
		}

		public void Populate()
		{
			var raceListCreator = new RaceListCreator();
			Entries = raceListCreator.MakeEntries();
		}
	}
}
