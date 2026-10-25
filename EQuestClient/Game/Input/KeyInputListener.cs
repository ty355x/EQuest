using System;
using Microsoft.Xna.Framework.Input;
using EQuestClient.Framework;

namespace EQuestClient.Game.Input
{
	public class KeyInputListener : IKeyInputListener	
	{
		private GameApp _game;
		private GameScene _scene;

		public bool MoveForwards = false;
		public bool MoveBackwards = false;
		public bool StrafeLeft = false;
		public bool StrafeRight = false;
		public bool LookLeft = false;
		public bool LookRight = false;

		public KeyInputListener(GameApp game, GameScene scene)
		{
			_game = game;
			_scene = scene;
		}

		public void OnKeyDown(Framework.KeyDownEventArgs args)
		{
			float elapsedTime = (float) args.Time.ElapsedGameTime.TotalSeconds;
			Keys key = args.Key;
			
			if (key == Keys.Space)
			{
				// Jump
				_scene.Player.MovementActions.Jump(args.Time);
			}
			else if (key == Keys.R)
			{
				// Run.
				_scene.Player.IsRunning = !_scene.Player.IsRunning;
			}
			else if (key == Keys.Z)
			{
				// Crouch.
				_scene.Player.MovementActions.ToggleCrouch(args.Time);
			}
			else if (key == Keys.Enter)
			{
				// Console input.
				_scene.Input.StartUICaptureInput();
			}
			else if (key == Keys.Escape)
			{
				// Cancel action.
				_scene.Player.CancelAction();
			}
			else if (key == Keys.I)
			{
				// Show inventory.
				_scene.UI.IsInventoryVisible = !_scene.UI.IsInventoryVisible;
			}
			else if (key == Keys.OemTilde)
			{
				// Attack.
				_scene.Player.ToggleAttack();
			}
			else if (key == Keys.F3)
			{
				// Noclip.
				_scene.NoClip = !_scene.NoClip;
			}
			else if (key == Keys.F9)
			{
				// Toggle skybox.
				if (_scene.Assets.SkyBox.SkyIndex >= _scene.Assets.SkyBox.ModelCount - 2)
					_scene.Assets.SkyBox.SkyIndex = 0;
				else
					_scene.Assets.SkyBox.SkyIndex++;
			}
			else if (key == Keys.F12)
			{
				// Exit
				_game.Exit();
			}
		}

		public void OnKeyUp(Framework.KeyUpEventArgs args)
		{
			Keys key = args.Key;
			
			if (key == Keys.W || key == Keys.Up)
			{
				// Move forwards.
				MoveForwards = false;
			}
			else if (key == Keys.S || key == Keys.Down)
			{
				// Move back.
				MoveBackwards = false;
			}
			else if (key == Keys.A)
			{
				// Strafe left.
				StrafeLeft = false;
			}
			else if (key == Keys.D)
			{
				// Strafe right.
				StrafeRight = false;
			}
			else if (key == Keys.Q || key == Keys.Left)
			{
				// Look/Strafe left.
				StrafeLeft = false;
				LookLeft = false;
			}
			else if (key == Keys.E || key == Keys.Right)
			{
				StrafeRight = false;
				LookRight = false;
			}
		}
	}
}
