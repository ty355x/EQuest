using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("loottable_entries")]
	public class DbLootTableEntry
	{
		public int loottable_id { get; set; }
		public int lootdrop_id { get; set; }
		
		public int multiplier { get; set; }
		public int probability { get; set; }

		[NotMapped]
		public List<DBLootDropEntry> LootDrops { get; set; }
	}
}
