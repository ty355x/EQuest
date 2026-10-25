using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace EQuestServer
{
	public class ServerConfig
	{
		private const string ConfigFileName = "Config.json";
		public string DatabaseConnectionString { get; set; }

		public static ServerConfig Load()
		{
			string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config\\" + ConfigFileName);
			string json = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<ServerConfig>(json);
		}
	}
}
