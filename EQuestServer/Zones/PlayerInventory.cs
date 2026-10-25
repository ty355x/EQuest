using System;
using System.Collections.Generic;
using System.Linq;
using EQuestLib;
using EQuestServer.Repositories;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones
{
	public class PlayerInventory
	{
		private const int SlotCount = 29;

		private Mob _mob;
		public List<DbItem> CursorItems { get; set; }
		public DbItem[] Items { get; private set; }

		public PlayerInventory(Mob mob)
		{
			_mob = mob;
			CursorItems = new List<DbItem>();
			Items = new DbItem[SlotCount];
		}

		public List<DbItem> GetUniqueItems()
		{
			var itemIds = new Dictionary<int, int>();
			var items = new List<DbItem>();

			foreach (var item in CursorItems)
			{
				if (!itemIds.ContainsKey(item.id))
				{
					itemIds.Add(item.id, item.id);
					items.Add(item);
				}
			}
			
			foreach (var item in Items)
			{
				if (item != null && !itemIds.ContainsKey(item.id))
				{
					itemIds.Add(item.id, item.id);
					items.Add(item);
				}
			}

			return items;
		}

		public void WriteToPacket(PacketOut packet)
		{
			int itemCount = CursorItems.Count + Items.Where(i => i != null).Count();
			packet.WriteInt(itemCount);

			foreach (var item in CursorItems)
			{
				packet.WriteInt(-1);
				packet.WriteInt(item.id);
			}

			for (int i = 0; i < Items.Length; i++)
			{
				if (Items[i] != null)
				{
					packet.WriteInt(i);
					packet.WriteInt(Items[i].id);
				}
			}
		}

		public void LoadInventory()
		{
			// Clear the cursor and items.
			CursorItems.Clear();

			for (int i = 0; i < Items.Length; i++)
				Items[i] = null;

			List<DbInventory> inven;

			using (var inventoryRepository = new InventoryRepository())
				inven = inventoryRepository.FindForCharacter(_mob.Client.Character.id);

			foreach (var invenItem in inven)
			{
				var item = GameData.FindItem(invenItem.item_id);

				if (item == null)
					continue;

				if (invenItem.slot_id == -1)
				{
					CursorItems.Add(item);
				} 
				else if (invenItem.slot_id >= 0 && invenItem.slot_id <= SlotCount)
				{
					Items[invenItem.slot_id] = item;
				}
			}
		}

		public void SaveInventory()
		{
			if (_mob.Client == null || _mob.Client.Character == null || _mob.Client.Character.id == 0)
				return;
			
			var characterId = _mob.Client.Character.id;
			var inventory = new List<DbInventory>();

			// Add cursor items.
			foreach (var item in CursorItems)
			{
				var inventoryItem = new DbInventory()
				{
					character_id = characterId,
					slot_id = -1,
					bag_slot_id = -1,
					item_id = item.id,

				};

				inventory.Add(inventoryItem);
			}

			// Add inventory items.
			for (int i = 0; i < Items.Length; i++)
			{
				var item = Items[i];

				if (item == null)
					continue;

				var inventoryItem = new DbInventory()
				{
					character_id = characterId,
					slot_id = i,
					bag_slot_id = -1,
					item_id = item.id,
				};

				inventory.Add(inventoryItem);
			}

			// Update the database.
			using (var inventoryRepository = new InventoryRepository())
				inventoryRepository.SaveForCharacter(characterId, inventory);
		}

		public void PickupItem(int slot)
		{
			if (slot >= SlotCount)
				return;

			var cursorItem = CursorItems.Count == 0 ? null : CursorItems[0];
			var slotItem = Items[slot];

			Items[slot] = cursorItem;

			if (CursorItems.Count > 0)
				CursorItems.RemoveAt(0);

			if (slotItem != null)
				CursorItems.Add(slotItem);
		}

		public void DestroyItem()
		{
			if (CursorItems.Count > 0)
				CursorItems.RemoveAt(0);
		}
	}
}
