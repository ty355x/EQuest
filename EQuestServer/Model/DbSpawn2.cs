using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("spawn2")]
	public class DbSpawn2
	{
		[Key]
		public int id { get; set; }
		public int spawngroupID { get; set; }
		public string zone { get; set; }
		public int version { get; set; }
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }
		public float heading { get; set; }
		public int respawntime { get; set; }
		public DBSpawnGroup SpawnGroup { get; set; }
	}
}
