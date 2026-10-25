using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("characters")]
	public class DbCharacter
	{
		[Key]
		public int id { get; set; }
		public int account_id { get; set; }
		public string name { get; set; }
		public int race { get; set; }
		public int gender { get; set; }
	}
}
