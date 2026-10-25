using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("lootdrop_entries")]
	public class DBLootDropEntry
	{
		public int lootdrop_id { get; set; }
		public int item_id { get; set; }
		public int item_charges { get; set; }
		public int equip_item { get; set; }
		public int chance { get; set; }
	}
}
