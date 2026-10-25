using System;

namespace EQuestClient
{
	public class PlayerLogin
	{
		public string Email { get; private set; }
		public string Password { get; private set; }

		public static PlayerLogin ParseLogin(string[] args)
		{
			var login = new PlayerLogin();
			
			for (int i = 0; i < args.Length; i += 2)
			{
				if (i + 1 >= args.Length)
					break;

				string arg = args[i].ToLower();
				string val = args[i + 1];

				if (arg == "-email")
				{
					login.Email = val;
				}
				else if (arg == "-password")
				{
					login.Password = val;
				}
			}

			return login;
		}

		public void EnsureLoginValid()
		{
			if (!IsLoginValid())
			{
				throw new Exception("Missing login credentials.\nPlease log in using the " + GameApp.GameName + " Launcher application.");
			}
		}

		public bool IsLoginValid()
		{
			return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);
		}
	}
}
