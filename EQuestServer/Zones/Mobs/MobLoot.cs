using System;
using System.Collections.Generic;
using EQuestServer.Repositories;

namespace EQuestServer.Zones.Mobs
{
	struct DropProbability 
	{ 
		public DBLootDropEntry Drop { get; set; } 
		public int Probability { get; set; }
	}

	public class MobLootEntry
	{
		public int Index { get; set; }
		public DbItem Item { get; set; }
	}

	public class MobLoot
	{
		public int Cash { get; set; }
		public List<MobLootEntry> Entries { get; private set; }
		public bool IsLooted { get; set; }
		public Mob Mob { get; set; }
		
		public MobLoot(Mob mob)
		{
			this.Mob = mob;
			Cash = 0;
			Entries = new List<MobLootEntry>();
			IsLooted = false;
		}

		public List<DbItem> GetUniqueItems()
		{
			var itemIds = new Dictionary<int, int>();
			var items = new List<DbItem>();

			foreach (var entry in Entries)
			{
				if (!itemIds.ContainsKey(entry.Item.id))
				{
					itemIds.Add(entry.Item.id, entry.Item.id);
					items.Add(entry.Item);
				}
			}

			return items;
		}

		public void AddLoot(DbLootTable lootTable)
		{
			Cash = ZoneServer.Random.Next(lootTable.mincash, lootTable.maxcash);

			if (lootTable.Entries != null)
			{
				foreach (var tableEntry in lootTable.Entries)
				{
					if (tableEntry.LootDrops == null || tableEntry.LootDrops.Count == 0)
						continue;

					if (ZoneServer.Random.Next(0, 100) > tableEntry.probability)
						continue;
					
					for (int i = 0; i < tableEntry.multiplier; i++)
						AddLootForLootTableEntry(tableEntry);
				}
			}
		}

		public void Clear()
		{
			Entries.Clear();
		}

		public void LootItem(Mob player, int itemIndex)
		{
			MobLootEntry lootEntry = null;

			foreach (var entry in Entries)
			{
				if (entry.Index == itemIndex)
				{
					lootEntry = entry;
					break;
				}
			}

			if (lootEntry != null)
			{
				Entries.Remove(lootEntry);
				player.PlayerInventory.CursorItems.Add(lootEntry.Item);
				player.Client.Send(PacketMaker.MakePickupLootItem(Mob.Id, itemIndex, lootEntry.Item.id));
			}
		}

		private void AddLootForLootTableEntry(DbLootTableEntry entry)
		{
			// Make probability list.
			int currentProbability = 0;
			var dropProbabilities = new DropProbability[entry.LootDrops.Count];

			for (int i = 0; i < entry.LootDrops.Count; i++)
			{
				var drop = entry.LootDrops[i];
				currentProbability += drop.chance;

				dropProbabilities[i] = new DropProbability()
				{
					Drop = drop,
					Probability = currentProbability
				};
			}

			DBLootDropEntry droppedItem = null;
			int itemChance = ZoneServer.Random.Next(1, 100);

			foreach (var drop in dropProbabilities)
			{
				if (drop.Probability < itemChance)
				{
					droppedItem = drop.Drop;
					continue;
				}
			}

			if (droppedItem == null)
				droppedItem = dropProbabilities[0].Drop;

			var item = GameData.FindItem(droppedItem.item_id);

			if (item != null)
			{
				int index = Entries.Count;
				Entries.Add(new MobLootEntry() { Index = index, Item = item });
			}
		}
	}
}
