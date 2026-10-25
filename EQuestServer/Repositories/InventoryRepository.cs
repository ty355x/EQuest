using System;
using System.Collections.Generic;
using System.Linq;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Repositories
{
	public class InventoryRepository : BaseRepository
	{
		public List<DbInventory> FindForCharacter(int characterId)
		{
			return _context.Inventory.Where(i => i.character_id == characterId).ToList();
		}

		public void SaveForCharacter(int characterId, List<DbInventory> inventory)
		{
			var oldInventory = FindForCharacter(characterId);

			_context.Inventory.RemoveRange(oldInventory);
			_context.SaveChanges();
			_context.Inventory.AddRange(inventory);
			_context.SaveChanges();
		}
	}
}
