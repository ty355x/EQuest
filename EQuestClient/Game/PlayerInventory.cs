using System;

namespace EQuestClient.Game
{
	public class PlayerInventory
	{
		public const int SlotCount = 29;

		public Item[] Items { get; private set; }
		private GameScene _scene;

		public PlayerInventory(GameScene scene)
		{
			Items = new Item[SlotCount];
			_scene = scene;
		}

		public void Clear()
		{
			for (int i = 0; i < Items.Length; i++)
				Items[i] = null;
		}

		public void Add(int slot, int itemId)
		{
			if (slot < 0 || slot >= Items.Length)
				return;
			
			var item = _scene.KnownItems.FindItem(itemId);
			Items[slot] = item;
		}

		public void PickupItem(int slot)
		{
			var cursorItem = _scene.Player.Cursor.Items.Count == 0 ? null : _scene.Player.Cursor.Items[0];
			var slotItem = Items[slot];

			Items[slot] = (Item) cursorItem;

			if (_scene.Player.Cursor.Items.Count > 0)
				_scene.Player.Cursor.Items.RemoveAt(0);

			if (slotItem != null)
				_scene.Player.Cursor.Items.Add(slotItem);
		}

		public void DestroyItem()
		{
			if (_scene.Player.Cursor.HasItems())
				_scene.Player.Cursor.RemoveAt(0);
		}
	}
}
