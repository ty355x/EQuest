using System;
using System.Collections.Generic;

namespace EQuestLib.Data
{
	public class RaceListCreator
	{
		private List<RaceInfo> _entries = new List<RaceInfo>();
		private const float DefaultRaceSize = 1.15f;

		public List<RaceInfo> MakeEntries()
		{
			AddEntries();
			return _entries;
		}

		private void AddEntries()
		{
			AddEntry("Soldier", "HUM");                 // 0
			AddEntry("Human", "HUM", "HUM", "HUF");
			AddEntry("Barbarian", "BAM", "BAM", "BAF");
			AddEntry("Erudite", "ERM", "ERM", "ERF");
			AddEntry("Wood Elf", "ELM", "ELM", "ELF");
			AddEntry("High Elf", "HIM", "HIM", "HIF");  // 5
			AddEntry("Dark Elf", "DAM", "DAM", "DAF");
			AddEntry("Half Elf", "HAM", "HAM", "HAF");
			AddEntry("Dwarf", "DWM", "DWM", "DWF");
			AddEntry("Troll", "TRM", "TRM", "TRF");
			AddEntry("Ogre", "OGM", "OGM", "OGF");  // 10
			AddEntry("Halfling", "HLM", "HLM", "HLF");
			AddEntry("Gnome", "GNM", "GNM", "GNF");
			AddEntry("Aviak", "AVI");
			AddEntry("WereWolf", "KOB");
			AddEntry("Brownie", "HUM");             // 15
			AddEntry("Centaur", "HUM");
			AddEntry("Golem", "HUM");
			AddEntry("Giant", "GIA");
			AddEntry("Trakanon", "HUM");
			AddEntry("Doppleganger", "HUM");        // 20
			AddEntry("Evil Eye", "HUM");
			AddEntry("Beetle", "BET");
			AddEntry("Kerra", "HUM");
			AddEntry("Fish", "FIS", 0.25f);
			AddEntry("Fairy", "HUM");               // 25
			AddEntry("Froglok", "FRO");
			AddEntry("Froglok Ghoul", "FRO");
			AddEntry("Fungusman", "HUM");
			AddEntry("Gargoyle", "GAR");
			AddEntry("Gasbag", "HUM");              // 30
			AddEntry("Gelatinous Cube", "HUM");
			AddEntry("Ghost", "HUM");
			AddEntry("Ghoul", "GOB");
			AddEntry("Giant Bat", "BAT");
			AddEntry("Giant Eel", "HUM");               // 35
			AddEntry("Giant Rat", "RAT");
			AddEntry("Giant Snake", "SNA");
			AddEntry("Spider", "SPI");
			AddEntry("Gnoll", "GNN");
			AddEntry("Goblin", "GOB");              // 40
			AddEntry("Gorilla", "HUM");
			AddEntry("Wolf", "WOL");
			AddEntry("Bear", "BEA");
			AddEntry("Freeport Guard", "HUM");
			AddEntry("Demi Lich", "HUM");               // 45
			AddEntry("Imp", "IMP");
			AddEntry("Griffin", "GRI");
			AddEntry("Kobold", "KOB");
			AddEntry("Lava Dragon", "DRA", 10);
			AddEntry("Lion", "HUM");                // 50
			AddEntry("Lizard Man", "LIZ");
			AddEntry("Mimic", "MIM", 0.05f);
			AddEntry("Minotaur", "HUM");
			AddEntry("Orc", "ORC");
			AddEntry("Human Beggar", "BGM");                // 55
			AddEntry("Pixie", "PIM", "PIM", "PIF");
			AddEntry("Dracnid", "HUM");
			AddEntry("Solusek Ro", "HUM");
			AddEntry("Blood Gill", "HUM");
			AddEntry("Skeleton", "SKE");            // 60
			AddEntry("Shark", "SHA");
			AddEntry("Tunare", "HUM");
			AddEntry("Tiger", "HUM");
			AddEntry("Treant", "TRE");
			AddEntry("Vampire", "DVM");             // 65
			AddEntry("Rallos Zek", "HUM");
			AddEntry("Highpass Citizen", "HUM");
			AddEntry("Tentacle", "HUM");
			AddEntry("Wisp", "WIL", 0.03f);
			AddEntry("Zombie", "ZOM");             // 70
			AddEntry("Qeynos Citizen", "QCM", "QCM", "QCF");
			AddEntry("Ship", "HUM");
			AddEntry("Launch", "HUM");
			AddEntry("Piranha", "PIR");
			AddEntry("Elemental", "HUM");               // 75
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Bixie", "BIX");
			AddEntry("Soldier", "HUM");         // 80
			AddEntry("Rivervale Citizen", "RIM", "RIM", "RIF");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Spectre", "SPE");         // 85
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Drake", "DRK");
			AddEntry("Soldier", "HUM");         // 90
			AddEntry("Alligator", "ALL");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Cazic Thule", "CAZ");         // 95
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Elf Vampire", "VSM", "VSM", "VSF");
			AddEntry("Soldier", "HUM");
			AddEntry("Dervish", "HUM");         // 100
			AddEntry("Efreeti", "EFR");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Swordfish", "SWO");         // 105
			AddEntry("Soldier", "HUM");
			AddEntry("Mammoth", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Mermaid", "MER");         // 110
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");         // 115
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");         // 120
			AddEntry("Soldier", "HUM");
			AddEntry("Soldier", "HUM");
			AddEntry("Innoruuk", "INN");
			AddEntry("Unicorn", "UNI");
			AddEntry("Soldier", "HUM");         // 125
		}

		private void AddEntry(string name, string neuterModel)
		{
			AddEntry(name, neuterModel, null, null, DefaultRaceSize);
		}

		private void AddEntry(string name, string neuterModel, float size)
		{
			AddEntry(name, neuterModel, null, null, size);
		}

		private void AddEntry(string name, string neuterModel, string maleModel, string femaleModel)
		{
			AddEntry(name, neuterModel, maleModel, femaleModel, DefaultRaceSize);
		}

		private void AddEntry(string name, string neuterModel, string maleModel, string femaleModel, float size)
		{
			var entry = new RaceInfo
			{
				Name = name,
				NeuterModel = neuterModel,
				MaleModel = maleModel != null ? maleModel : neuterModel,
				FemaleModel = femaleModel != null ? femaleModel : neuterModel,
				Size = size
			};

			_entries.Add(entry);
		}
	}
}
