using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using EQuestClient.Framework;

namespace EQuestClient.Game.Input
{
	public class UIKeyInputListener : IKeyInputListener
	{
		private GameScene _scene;
		private List<KeyMap> _keyMap = new List<KeyMap>();

		public UIKeyInputListener(GameScene scene)
		{
			_scene = scene;
			BuildKeyMap();
		}

		public void OnKeyDown(Framework.KeyDownEventArgs args)
		{
			if (args.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
				_scene.Input.SubmitUIInput();
			else
				ProcessConsoleInput(args);
		}

		public void OnKeyUp(Framework.KeyUpEventArgs args)
		{
		}

		private void ProcessConsoleInput(Framework.KeyDownEventArgs args)
		{
			// Check for escape.
			if (args.Key == Keys.Escape)
			{
				_scene.Input.CancelUICaptureInput();
				return;
			}

			// Check for backspace.
			if (args.Key == ((Keys)8))
			{
				_scene.UI.Console.AddConsoleInput(8);
				return;
			}

			// Check for word characters.
			for (int i = 65; i <= 90; i++)
			{
				if (args.Key == ((Keys)i))
				{
					if (args.Shift)
						_scene.UI.Console.AddConsoleInput(i);
					else
						_scene.UI.Console.AddConsoleInput(i + 32);

					return;
				}
			}

			// Check for other printable characters.
			foreach (var keyMap in _keyMap)
			{
				if (args.Key == (keyMap.ScanCode))
				{
					if (args.Shift)
						_scene.UI.Console.AddConsoleInput(keyMap.KeyAlt);
					else
						_scene.UI.Console.AddConsoleInput(keyMap.Key);

					return;
				}
			}
		}

		private void BuildKeyMap()
		{
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemTilde, Key = "`", KeyAlt = "~" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D1, Key = "1", KeyAlt = "!" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D2, Key = "2", KeyAlt = "@" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D3, Key = "3", KeyAlt = "#" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D4, Key = "4", KeyAlt = "$" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D5, Key = "5", KeyAlt = "%" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D6, Key = "6", KeyAlt = "^" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D7, Key = "7", KeyAlt = "&" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D8, Key = "8", KeyAlt = "*" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D9, Key = "9", KeyAlt = "(" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.D0, Key = "0", KeyAlt = ")" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemMinus, Key = "-", KeyAlt = "_" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemPlus, Key = "=", KeyAlt = "+" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemOpenBrackets, Key = "[", KeyAlt = "{" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemCloseBrackets, Key = "]", KeyAlt = "}" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemPipe, Key = "\\", KeyAlt = "|" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemSemicolon, Key = ";", KeyAlt = ":" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemQuotes, Key = "'", KeyAlt = "\"" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemComma, Key = ",", KeyAlt = "<" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemPeriod, Key = ".", KeyAlt = ">" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.OemQuestion, Key = "/", KeyAlt = "?" });
			_keyMap.Add(new KeyMap { ScanCode = Keys.Space, Key = " ", KeyAlt = " " });
		}
	}
}
