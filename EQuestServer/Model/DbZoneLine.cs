using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EQuestServer.Repositories
{
	[Table("zone_points")]
	public class DbZoneLine
	{
		[Key]
		public int id { get; set; }
		public string zone { get; set; }
		public int number { get; set; }
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }
		public float heading { get; set; }
		public float target_x { get; set; }
		public float target_y { get; set; }
		public float target_z { get; set; }
		public float target_heading { get; set; }
		public int target_zone_id { get; set; }
	}
}
