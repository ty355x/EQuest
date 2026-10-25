using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("spawnentry")]
	public class DbSpawnEntry
	{
		public int spawngroupID { get; set; }
		public int npcID { get; set; }
		public int chance { get; set; }
	}
}
