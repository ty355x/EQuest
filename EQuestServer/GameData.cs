using System;
using System.Collections.Generic;
using System.Linq;
using EQuestServer.Repositories;

namespace EQuestServer
{
	/// <summary>
	/// Manages shared data for the login server and zones.
	/// </summary>
	public static class GameData
	{
		private static Dictionary<int, DbZone> zones = new Dictionary<int, DbZone>();
		private static Dictionary<int, DbItem> items = new Dictionary<int, DbItem>();

		public static void LoadData()
		{
			Log.Info("Loading shared game data...");

			using (var zoneRepository = new ZoneRepository())
			{
				var zoneList = zoneRepository.FindAll();

				foreach (var zone in zoneList)
					zones[zone.zoneidnumber] = zone;
			}

			using (var itemRepository = new DBItemRepository())
			{
				var itemList = itemRepository.FindAll();

				foreach (var item in itemList)
					items[item.id] = item;
			}

			Log.Info(string.Format("Loaded {0} zones, {1} items", zones.Count, items.Count));
		}

		public static DbZone FindZone(int id)
		{
			return zones.ContainsKey(id) ? zones[id] : null;
		}

		public static DbZone FindZone(string name)
		{
			return zones.Values.Where(z => z.short_name == name).FirstOrDefault();
		}

		public static DbItem FindItem(int id)
		{
			return items.ContainsKey(id) ? items[id] : null;
		}
	}
}
