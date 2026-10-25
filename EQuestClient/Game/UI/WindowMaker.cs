using System;
using Microsoft.Xna.Framework;
using EQuestClient.UI;

namespace EQuestClient.Game.UI
{
	public class WindowMaker
	{
		private GameApp _game;
		private GameScene _scene;
		private GameSceneUI _ui;
		private Viewport _viewport;

		public WindowMaker(GameApp game, GameScene scene, GameSceneUI ui, Viewport desktop)
		{
			_game = game;
			_scene = scene;
			_ui = ui;
			_viewport = desktop;
		}

		public ConsoleWindow MakeConsoleWindow()
		{
			var window = new ConsoleWindow();
			window.Width = Math.Min((int)(_viewport.Width * 0.40), 720);
			window.Height = Math.Min((int)(_viewport.Height * 0.25), 300);
			return window;
		}

		public PlayerWindow MakePlayerWindow()
		{
			var window = new PlayerWindow(_scene);
			window.Bounds = new Rectangle(0, 0, PlayerWindow.DefaultWidth, PlayerWindow.DefaultHeight);
			return window;
		}

		public PlayerTargetWindow MakePlayerTargetWindow(Window reference)
		{
			var window = new PlayerTargetWindow(_scene);
			window.Bounds = new Rectangle(0, 0, PlayerTargetWindow.DefaultWidth, PlayerTargetWindow.DefaultHeight);
			return window;
		}

		public InventoryWindow MakeInventoryWindow()
		{
			var window = new InventoryWindow(_scene);
			window.Bounds = new Rectangle(0, 0, InventoryWindow.DefaultWidth, InventoryWindow.DefaultHeight);
			return window;
		}

		public LootWindow MakeLootWindow(Window reference)
		{
			var window = new LootWindow(_game, _scene);
			window.Bounds = new Rectangle(0, 0, LootWindow.DefaultWidth, LootWindow.DefaultHeight);
			return window;
		}
	}
}
