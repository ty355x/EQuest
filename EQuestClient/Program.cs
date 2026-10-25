using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using EQuestConfig;

namespace EQuestClient
{
	static class Program
	{
		public static GameApp Game;

		private const string AssetsFolderName = "Assets";
		private const bool UseVR = false;

		[STAThread]
		static void Main(string[] args)
		{
 #if DEBUG
			StartGameDebug(args);
#else
			StartGameRelease(args);
#endif
		}

		private static void StartGame(string[] args)
		{
			// Get the config file.
			var configService = new GameConfigService();
			configService.EnsureConfigExists(GameApp.GameName);
			var config = configService.ReadConfig();

			// Parse the user login.
			var login = PlayerLogin.ParseLogin(args);
			login.EnsureLoginValid();

			// Get the assets path.
			string assetsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), AssetsFolderName);

			using (Game = new GameApp(login, config, assetsPath, UseVR))
			{
				Game.Run();
			}
		}

		private static void StartGameRelease(string[] args)
		{
			try
			{
				StartGame(args);
			}
			catch (Exception e)
			{
				if (Program.Game != null)
					Program.Game.Exit();
				
				MessageBox.Show(e.Message, "EQuest Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void StartGameDebug(string[] args)
		{
			StartGame(args);
		}

		private static void EnsureAssetsPathExists(string assetsPath)
		{
			if (!Directory.Exists(assetsPath))
			{
				throw new Exception("Game assets path not found");
			}
		}
	}
}

