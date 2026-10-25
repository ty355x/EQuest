using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EQuestServer.Repositories
{
	public class NpcTypeRepository : BaseRepository
	{
		public DbNpcType Find(int id)
		{
			// Load the NPC type.
			var npcType = _context.NpcTypes
				.Where(n => n.id == id)
				.Include(n => n.LootTable)
				.FirstOrDefault();

			if (npcType != null)
			{
				// Load loot table entries.
				if (npcType.LootTable != null)
				{
					npcType.LootTable.Entries = _context.LootTableEntries
						.Where(e => e.loottable_id == npcType.loottable_id)
						.ToList();
				}

				// Load loot drop entries.
				if (npcType.LootTable != null && npcType.LootTable.Entries.Count > 0)
				{
					foreach (var entry in npcType.LootTable.Entries)
					{
						entry.LootDrops = _context.LootDropEntries
							.Where(d => d.lootdrop_id == entry.lootdrop_id)
							.ToList();
					}
				}
			}

			return npcType;
		}
	}
}
