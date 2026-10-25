using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("npc_types")]
	public class DbNpcType
	{
		[Key]
		public int id { get; set; }
		public string name { get; set; }
		public string lastname { get; set; }
		public int level { get; set; }
		public int race { get; set; }
		public int gender { get; set; }
		public float size { get; set; }
		public int texture { get; set; }
		public int hp { get; set; }
		public int mindmg { get; set; }
		public int maxdmg { get; set; }

		[ForeignKey("LootTable")]
		public int? loottable_id { get; set; }
		public virtual DbLootTable LootTable { get; set; }
	}
}
