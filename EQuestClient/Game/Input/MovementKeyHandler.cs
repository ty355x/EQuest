using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EQuestClient.Game.Input
{
	public class MovementKeyHandler
	{
		private const float TurnRate = 120.0f;

		private GameApp _game;
		private GameScene _scene;
		private KeyboardState _keyboardState;
		
		public MovementKeyHandler(GameApp game, GameScene scene)
		{
			_game = game;
			_scene = scene;
		}
		
		public void Update(GameTime time)
		{
			_keyboardState = Keyboard.GetState();
			float elapsedTime = (float)time.ElapsedGameTime.TotalSeconds;

			 _scene.Player.Movement.Move = Vector3.Zero;
			 _scene.Player.RotateDirection = 0;

			// Move forwards.
			if ((IsKeyDown(Keys.W) && !_scene.Input.IsUICapturingInput) || IsKeyDown(Keys.Up))
				_scene.Player.Movement.Move.Y--;
			
			// Move backwards.
			if ((IsKeyDown(Keys.S) && !_scene.Input.IsUICapturingInput) || IsKeyDown(Keys.Down))
				_scene.Player.Movement.Move.Y++;

			// Strafe left.
			if (IsKeyDown(Keys.A) && !_scene.Input.IsUICapturingInput)
				 _scene.Player.Movement.Move.X++;

			// Strafe right.
			if (IsKeyDown(Keys.D) && !_scene.Input.IsUICapturingInput)
				_scene.Player.Movement.Move.X--;

			// Look/Strafe left.
			if ((IsKeyDown(Keys.Q) && !_scene.Input.IsUICapturingInput) || IsKeyDown(Keys.Left))
			{
				if (IsShiftDown())
				{
					_scene.Player.Movement.Move.X++;
				}
				else
				{
					_scene.Player.Heading += (TurnRate * elapsedTime);
					_scene.Player.RotateDirection = 1;
				}
			}

			// Look/Strafe right.
			if ((IsKeyDown(Keys.E) && !_scene.Input.IsUICapturingInput) || IsKeyDown(Keys.Right))
			{
				if (IsShiftDown())
				{
					_scene.Player.Movement.Move.X--;
				}
				else
				{
					_scene.Player.Heading -= (TurnRate * elapsedTime);
					_scene.Player.RotateDirection = -1;
				}
			}

			// Look/Move up.
			if (IsKeyDown(Keys.PageUp))
			{
				if (IsShiftDown())
					_scene.Player.Movement.Move.Z++;
				else
					_scene.Player.Pitch += (TurnRate * elapsedTime);
			}

			// Look/Move down.
			if (IsKeyDown(Keys.PageDown))
			{
				if (IsShiftDown())
					_scene.Player.Movement.Move.Z--;
				else
					_scene.Player.Pitch -= (TurnRate * elapsedTime);
			}

			_scene.Player.Movement.Move.X = Math.Min(_scene.Player.Movement.Move.X, 1);
			_scene.Player.Movement.Move.X = Math.Max(_scene.Player.Movement.Move.X, -1);
			_scene.Player.Movement.Move.Y = Math.Min(_scene.Player.Movement.Move.Y, 1);
			_scene.Player.Movement.Move.Y = Math.Max(_scene.Player.Movement.Move.Y, -1);
			_scene.Player.Movement.Move.Z = Math.Min(_scene.Player.Movement.Move.Z, 1);
			_scene.Player.Movement.Move.Z = Math.Max(_scene.Player.Movement.Move.Z, -1);

			if (_scene.Player.Heading > 360)
				_scene.Player.Heading -= 360;
			else if (_scene.Player.Heading < 0)
				_scene.Player.Heading += 360;

			if (_scene.Player.Pitch > 89)
				_scene.Player.Pitch = 89;

			if (_scene.Player.Pitch < -89)
				_scene.Player.Pitch = -89;
		}

		private bool IsKeyDown(Keys key)
		{
			return _keyboardState.IsKeyDown(key);
		}

		private bool IsShiftDown()
		{
			return _keyboardState.IsKeyDown(Keys.LeftShift) || _keyboardState.IsKeyDown(Keys.RightShift);
		}
	}
}
