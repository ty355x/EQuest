using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("spawngroup")]
	public class DBSpawnGroup
	{
		[Key]
		public int id { get; set; }
		public string name { get; set; }
		
		[ForeignKey("spawngroupID")]
		public List<DbSpawnEntry> SpawnEntries { get; set; }
	}
}
