using System;
using Trace = System.Diagnostics.Trace;

namespace EQuestClient
{
    public class GameSettings2
    {
		public string AssetsPath { get; set; }
		public bool IsFullScreen { get; set; }
		public float MouseSpeed { get; set; }
		public string AccountEmail { get; set; }
		public string AccountPassword { get; set; }
		public int Race { get; set; }
		public int Gender { get; set; }
		public int ResolutionHeight { get; set; }
		public int ResolutionWidth { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public static bool UseVR { get; set; }

		public GameSettings2(string[] args)
        {
			// Set default settings.
			AssetsPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Assets");

			ResolutionWidth = 2560;
			ResolutionHeight = 1440;
			IsFullScreen = true;
			MouseSpeed = 0.25f;
			Race = 1;
			Gender = 0;
			Host = "127.0.0.1";
			Port = 1999;
			UseVR = false;

#if DEBUG
			AccountEmail = "ty@ty6.info";
			AccountPassword = "r00bies44";
			ResolutionWidth = 1920;
			ResolutionHeight = 1080;
			IsFullScreen = false;
			UseVR = false;
#endif
			// Parse command line arguments.
			ParseArgs(args);
        }
        
		private void ParseArgs(string[] args)
		{
			for (int i = 0; i < args.Length; i += 2)
			{
				if (i + 1 >= args.Length)
					break;

				string arg = args[i].ToLower();
				string val = args[i + 1];

				if (arg == "-resolution")
				{
					ParseResolution(val);
				}
				else if (arg == "-fullscreen")
				{
					IsFullScreen = (val == "true");
				}
				else if (arg == "-mousespeed")
				{
					ParseMouseSpeed(val);
				}
				else if (arg == "-email")
				{
					AccountEmail = val;
				}
				else if (arg == "-password")
				{
					AccountPassword = arg;
				}
				else if (arg == "-race")
				{
					Race = int.Parse(val);
				}
				else if (arg == "-gender")
				{
					Gender = int.Parse(val);
				}
				else if (arg == "-host")
				{
					Host = val;
				}
				else if (arg == "-port")
				{
					Port = int.Parse(val);
				}
			}
		}

		private void ParseResolution(string val)
		{
			if (string.IsNullOrEmpty(val))
				return;

			string[] split = val.Split('x');

			if (split.Length != 2)
				return;

			int widthParam = 320;
			int heightParam = 200;
			
			int.TryParse(split[0], out widthParam);
			int.TryParse(split[1], out heightParam);

			if (widthParam < 320 || heightParam < 200)
				return;

			ResolutionWidth = widthParam;
			ResolutionHeight = heightParam;
		}

		private void ParseMouseSpeed(string val)
		{
			float ms = 0;

			if (!float.TryParse(val, out ms))
			{
				Trace.WriteLine("Invalid mouse speed value");
				return;
			}

			if (ms < 0.10f || ms > 2.00f)
			{
				Trace.WriteLine("Mouse speed out of range 0.1 - 2.0");
				return;
			}

			MouseSpeed = ms;
		}
	}
}
