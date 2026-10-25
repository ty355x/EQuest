using System;
using Microsoft.Xna.Framework;
using EQuestRender;
using EQuestClient.UI;
using Console = EQuestClient.UI.Controls.Console;

namespace EQuestClient.Game.UI
{
	public class GameSceneUI
	{
		public Console Console { get; private set; }

		public bool IsInventoryVisible
		{
			get { return _inventoryWindow.IsVisible; }
			
			set 
			{
				_inventoryWindow.IsVisible = value;
				_lootWindow.IsVisible = false;
			
				if (value)
					LayoutInventoryWindow();
			}
		}

		public bool IsLootVisible
		{
			get { return _lootWindow.IsVisible; }
			
			set 
			{
				_inventoryWindow.IsVisible = value;
				_lootWindow.IsVisible = value;

				if (value)
					LayoutInventoryAndLootWindow();
			}
		}

		private PlayerWindow _playerWindow;
		private PlayerTargetWindow _playerTargetWindow;
		private ConsoleWindow _consoleWindow;
		private LootWindow _lootWindow = null;
		private InventoryWindow _inventoryWindow;
		private Viewport _viewport;

		private GameApp _game;
		private GameScene _scene;
		private WindowMaker _windowMaker;

		//
		// Constructor
		//

		public GameSceneUI(GameApp game, GameScene scene)
		{
			_game = game;
			_scene = scene;
		}

		//
		// Public Methods
		//

		public void Draw(Render render, GameTime time)
		{
			_viewport.Draw(render, time, _scene.Input.MousePosition.X, _scene.Input.MousePosition.Y);
		}

		public void Write(string text)
		{
			Console.Write(text);
		}

		public void Write(string text, Color color)
		{
			Console.Write(text, color);
		}

		public bool CheckClick(int x, int y)
		{
			return _viewport.CheckClick(x, y);
		}

		public void Initialize(int desktopWidth, int desktopHeight)
		{
			InitializeUIDefaults();
			_viewport = new Viewport(new Rectangle(0, 0, desktopWidth, desktopHeight));
			CreateWindows();
			LayoutWindows();
		}

		//
		// Private Methods
		//

		private void InitializeUIDefaults()
		{
			Viewport.DefaultTextFont = _scene.Assets.UI.DefaultFont;
			Viewport.DefaultBackground = _scene.Assets.UI.WindowBackground;
			Viewport.DefaultBackgroundTint = new Color(90, 90, 90, 120);
			Viewport.DefaultTransBackground = _scene.Assets.UI.TransFullImage;
			Viewport.DefaultProgressBackground = _scene.Assets.UI.Progress;
			Viewport.TooltipFont = _scene.Assets.UI.DefaultFont;
			Viewport.TooltipBackground = _scene.Assets.UI.WindowBackground;
			Viewport.ToolTipBackgroundTint = new Color(220, 220, 220, 220);
		}

		private void CreateWindows()
		{
			// Create the window maker.
			_windowMaker = new WindowMaker(_game, _scene, this, _viewport);

			// Add the console window.
			_consoleWindow = _windowMaker.MakeConsoleWindow();
			_viewport.AddWindow(_consoleWindow);
			Console = _consoleWindow.Console;
			Console.ConsoleTextSubmitted += OnConsoleTextSubmitted;

			// Add the player window.
			_playerWindow = _windowMaker.MakePlayerWindow();
			_viewport.AddWindow(_playerWindow);
			
			// Add the player target window.
			_playerTargetWindow = _windowMaker.MakePlayerTargetWindow(_playerWindow);
			_viewport.AddWindow(_playerTargetWindow);
			
			// Add the inventory window.
			_inventoryWindow = _windowMaker.MakeInventoryWindow();
			_inventoryWindow.IsVisible = false;
			_viewport.AddWindow(_inventoryWindow);

			// Add the loot window.
			_lootWindow = _windowMaker.MakeLootWindow(_inventoryWindow);
			_lootWindow.IsVisible = false;
			_viewport.AddWindow(_lootWindow);
		}

		private void LayoutWindows()
		{
			// Align the console window.
			WindowAlign.BottomLeft(_consoleWindow, _viewport);
			
			// Align the player window.
			WindowAlign.TopRight(_playerWindow, _viewport);

			// Align the player target window.
			WindowAlign.Right(_playerTargetWindow, _viewport);
			WindowAlign.BelowWindow(_playerTargetWindow, _playerWindow);
		}

		private void LayoutInventoryWindow()
		{
			WindowAlign.Center(_inventoryWindow, _viewport);
		}

		private void LayoutInventoryAndLootWindow()
		{
			WindowAlign.Center(_viewport, _inventoryWindow, _lootWindow);
		}

		//
		// Event Handlers
		//

		private void OnConsoleTextSubmitted(object sender, string text)
		{
			_scene.Network.SendMessage(text);
		}
	}
}
