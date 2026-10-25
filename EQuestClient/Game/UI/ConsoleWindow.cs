using System;
using EQuestClient.UI;
using Console = EQuestClient.UI.Controls.Console;

namespace EQuestClient.Game.UI
{
	public class ConsoleWindow : Window
	{
		public Console Console { get; private set; }
		
		public override void Initialize()
		{
			Console = new Console();
			Console.Bounds = Bounds;
			Console.DetermineMaxLines();
			AddChild(Console);
		}
	}
}
