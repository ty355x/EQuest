using System;
using Microsoft.Xna.Framework;

namespace EQuestServer
{
	public class ServerGameConfig
	{
		public string BootTestZone { get; set; }
		public string PlayerStartZone { get; set; }
		public Vector3 PlayerStartPos { get; set; }
		public float PlayerStartHeading { get; set; }
		
		private static ServerGameConfig _instance;

		public static ServerGameConfig Instance 
		{
			get 
			{
				if (_instance == null)
					_instance = new ServerGameConfig();

				return _instance;
			}
		}

		public ServerGameConfig()
		{
			BootTestZone = null;
			PlayerStartZone = "qeynos2";
			PlayerStartPos = new Vector3(450, 110, 0.88f);
			PlayerStartHeading = 360;
		}
	}
}
