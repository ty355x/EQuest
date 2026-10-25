using System;
using System.Collections.Generic;

namespace EQuestClient.Game
{
	/// <summary>
	/// Manages the list of known items that the client has received from the server.
	/// </summary>
	public class KnownItems
	{
		private Dictionary<int, Item> _items = new Dictionary<int, Item>();
		private GameScene _scene;

		public KnownItems(GameScene scene)
		{
			_scene = scene;
		}		
		
		public void AddItem(Item item)
		{
			if (!_items.ContainsKey(item.ID))
			{
				// If the item does not exist, add it to the dictionary.
				item.IsKnown = true;
				_items.Add(item.ID, item);
			} 
			else
			{
				// Otherwise update the item if it is unknown.
				if (_items[item.ID].IsKnown == false)
					_items[item.ID] = item;
			}
		}

		public Item FindItem(int id)
		{
			// If the item is not in the dictionary, return an unknown item to prevent a crash.
			if (!_items.ContainsKey(id))
			{
				var unknownItem = new Item(_scene);
				unknownItem.ID = id;
				unknownItem.Name = "Item";
				_items.Add(id, unknownItem);
			}

			return _items[id];
		}
	}
}
