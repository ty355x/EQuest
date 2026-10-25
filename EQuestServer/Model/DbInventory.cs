using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("inventory")]
	public class DbInventory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int character_id { get; set; }
		public int slot_id { get; set; }
		public int bag_slot_id { get; set; }
		public int item_id { get; set; }
	}
}
