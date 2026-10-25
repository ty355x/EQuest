using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("accounts")]
	public class DbAccount
	{
		[Key]
		public int id { get; set; }
		public string email { get; set; }
		public string password { get; set; }
	}
}
