using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("zone")]
	public class DbZone
	{
		[Key]
		public int zoneidnumber { get; set; }
		public string short_name { get; set; }
		public string long_name { get; set; }
		public float safe_x { get; set; }
		public float safe_y { get; set; }
		public float safe_z { get; set; }
	}
}
