using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("items")]
	public class DbItem
	{
		[Key]
		public int id { get; set; }
		public string name { get; set; }
		public int icon { get; set; }
		public string idfile { get; set; }
	}
}
