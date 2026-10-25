using System;
using System.Collections.Generic;
using EQuestClient.Network;

namespace EQuestClient.Game
{
	public class LootItem
	{
		public int Index { get; set; }
		public Item Item { get; set; }

		public LootItem(int index, Item item)
		{
			this.Index = index;
			this.Item = item;
		}
	}
	
	public class PlayerLooting
	{
		public bool IsLooting { get; set; }
		public Mob Mob { get; set; }
		public List<LootItem> Loot { get; private set; }
				
		private GameScene _scene;

		public PlayerLooting(GameScene scene)
		{
			_scene = scene;
			Loot = new List<LootItem>();
		}

		public void RequestPickupLoot(int itemIndex)
		{
			if (_scene.Player.Cursor.HasItems() || itemIndex < 0 || itemIndex > Loot.Count)
				return;

			_scene.Network.SendPacket(PacketMaker.MakeLootItems(Mob.ID, itemIndex));
		}

		public void PickupItem(int corpseId, int itemIndex, int itemId)
		{
			LootItem lootItem = null;

			if (Mob.ID == corpseId)
			{
				foreach (var lootItemSearch in Loot)
				{
					if (lootItemSearch.Index == itemIndex)
					{
						lootItem = lootItemSearch;
						break;
					}
				}
			}

			if (lootItem != null)
				Loot.Remove(lootItem);
			
			var item = _scene.KnownItems.FindItem(itemId);

			if (item != null)
				_scene.Player.Cursor.Add(item);
		}

		public void StartLooting(int mobId, int cash, List<LootItem> lootItems)
		{
			EndLooting();
			
			var mob = _scene.Zone.FindMob(mobId);

			if (mob == null)
				return;
			
			Mob = mob;

			if (cash > -1)
				_scene.UI.Write("You looted " + cash + " cash.");

			IsLooting = true;
			Loot = lootItems;

			foreach (var item in Loot)
				_scene.KnownItems.AddItem(item.Item);
			
			_scene.UI.IsLootVisible = true;
		}

		public void EndLooting()
		{
			IsLooting = false;
			Mob = null;
			Loot.Clear();
			_scene.UI.IsLootVisible = false;
		}
	}
}
