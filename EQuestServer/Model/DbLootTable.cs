using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("loottable")]
	public class DbLootTable
	{
		[Key]
		public int id { get; set; }
		public string name { get; set; }
		public int mincash { get; set; }
		public int maxcash { get; set; }

		[NotMapped]
		public List<DbLootTableEntry> Entries { get; set; }
	}
}
