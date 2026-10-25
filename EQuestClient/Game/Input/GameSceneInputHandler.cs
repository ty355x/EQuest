using System;
using Microsoft.Xna.Framework;
using EQuestClient.Framework;

namespace EQuestClient.Game.Input
{
	public class GameSceneInputHandler
	{
		public bool IsUICapturingInput { get; private set; }
		
		public Point MousePosition { get => _mouseInputHandler.MousePosition; }
		public float MouseSpeed { get => _mouseInputHandler.MouseSpeed; set => _mouseInputHandler.MouseSpeed = value; }
		
		private KeyInputHandler _keyInputHandler;
		private GameSceneMouseInputHandler _mouseInputHandler;
		private MovementKeyHandler _movementKeyHandler;
		private KeyInputListener _keyInputListener;
		private UIKeyInputListener _uiKeyInputListener;

		private GameScene _scene;
		
		public GameSceneInputHandler(GameApp game, GameScene scene)
		{
			_scene = scene;
			
			_keyInputHandler = new KeyInputHandler();
			_mouseInputHandler = new GameSceneMouseInputHandler(game, _scene);
			_movementKeyHandler = new MovementKeyHandler(game, _scene);
			_keyInputListener = new KeyInputListener(game, _scene);
			_uiKeyInputListener = new UIKeyInputListener(_scene);

			_keyInputHandler.Listener = _keyInputListener;
			IsUICapturingInput = false;
		}

		public void Update(GameTime time)
		{
			_mouseInputHandler.Update(time);
			_movementKeyHandler.Update(time);
			_keyInputHandler.Update(time);
		}

		public void StartUICaptureInput()
		{
			_scene.Player.Movement.StopMovement(false);
			_scene.UI.Console.StartConsoleCapture();
			_keyInputHandler.Listener = _uiKeyInputListener;
			IsUICapturingInput = true;
		}

		public void SubmitUIInput()
		{
			_scene.UI.Console.SubmitConsoleInput();
			CancelUICaptureInput();
		}

		public void CancelUICaptureInput()
		{
			_scene.UI.Console.CancelConsoleInput();
			_keyInputHandler.Listener = _keyInputListener;
			IsUICapturingInput = false;
		}
	}
}
