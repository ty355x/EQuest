using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EQuestClient.Framework;
using EQuestClient.UI;

namespace EQuestClient.Test.TestMobs
{
	public class TestMobsInputHandler : IKeyInputListener, IMouseInputListener	
	{
		private GameApp _game;
		private TestMobsScene _scene;
		private Viewport _viewport;
		private KeyInputHandler _keyInputHandler = new KeyInputHandler();
		private MouseInputHandler _mouseInputHandler = new MouseInputHandler();
	
		public TestMobsInputHandler(GameApp game, TestMobsScene scene, Viewport viewport)
		{
			_game = game;
			_scene = scene;
			_viewport = viewport;
		}

		public void Initialize()
		{
			_keyInputHandler.Listener = this;
			_mouseInputHandler.Listener = this;
		}

		public void Update(GameTime time)
		{
			_keyInputHandler.Update(time);
			_mouseInputHandler.Update(time);
		}

		public void OnKeyDown(KeyDownEventArgs args)
		{
			if (args.Key == Keys.F12)
				_game.Exit();
			else if (args.Key == Keys.Left)
				_scene.ShowPreviousFrame();
			else if (args.Key == Keys.Right)
				_scene.ShowNextFrame();
		}

		public void OnKeyUp(KeyUpEventArgs args)
		{
		}

		public void OnMouseButtonPressed(MouseEventArgs args)
		{
			if (args.Button == MouseButtons.LeftButton)
				_viewport.CheckClick(args.Position.X, args.Position.Y);
		}

		public void OnMouseButtonReleased(MouseEventArgs args)
		{
		}
	}
}
