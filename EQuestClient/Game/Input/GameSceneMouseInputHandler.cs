using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EQuestClient.Game.Input
{
	public class GameSceneMouseInputHandler
	{
		private const float TurnRate = 120.0f;
		public Point MousePosition { get; private set; }

		public float MouseSpeed { get; set; }
		
		private GameApp _game = null;
		private GameScene _scene = null;
		private bool[] _pressedKeys = null;
		
		private bool _captureMouse = false;
		public Point _currentMousePositon = Point.Zero;
		private Point _mouseOrigin = Point.Zero;

		private MouseState _lastMouseState;

		public GameSceneMouseInputHandler(GameApp game, GameScene scene)
		{
			_game = game;
			_scene = scene;
			_pressedKeys = new bool[256];
			_currentMousePositon = Point.Zero;
			MouseSpeed = 0.35f;
			_lastMouseState = Mouse.GetState();
			_mouseOrigin = new Point(_game.GraphicsDevice.Viewport.Width / 2, _game.GraphicsDevice.Viewport.Height / 2);
		}

		public void Update(GameTime time)
		{
			if (!_scene.Player.IsFrozen)
				CheckMouseState(time);
		}

		private void CheckMouseState(GameTime time)
		{
			var mouseState = Mouse.GetState();
			MousePosition = mouseState.Position;
			bool skipMouseLook = false;

			if (mouseState.RightButton != _lastMouseState.RightButton)
			{
				if (mouseState.RightButton == ButtonState.Pressed)
				{
					_captureMouse = true;
					_game.IsMouseVisible = false;
					_currentMousePositon = mouseState.Position;
					ResetMouse();
					skipMouseLook = true;
				}
				else
				{
					_captureMouse = false;
					_game.IsMouseVisible = true;
					Mouse.SetPosition(_currentMousePositon.X, _currentMousePositon.Y);
				}
			}
			
			if (!skipMouseLook && _captureMouse && mouseState.Position != _mouseOrigin)
			{
				float deltaX = mouseState.X - _mouseOrigin.X;
				float deltaY = mouseState.Y - _mouseOrigin.Y;

				_scene.Player.Heading -= deltaX * MouseSpeed;
				_scene.Player.Pitch -= deltaY * MouseSpeed;

				ResetMouse();
			}

			if (!_captureMouse && mouseState.LeftButton != _lastMouseState.LeftButton && mouseState.LeftButton == ButtonState.Pressed)
			{
				if (!_scene.UI.CheckClick(mouseState.X, mouseState.Y))
					_scene.Player.Targetting.SelectTarget(mouseState.Position);
			}

			if (mouseState.RightButton != _lastMouseState.RightButton && mouseState.RightButton == ButtonState.Pressed)
				_scene.Player.Targetting.Interact(mouseState.Position);

			_lastMouseState = mouseState;
		}

		private void ResetMouse()
		{
			Mouse.SetPosition(_mouseOrigin.X, _mouseOrigin.Y);
		}
	}
}
