using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EQuestClient.Framework
{
	public class KeyInputHandler
	{
		public IKeyInputListener Listener { get; set; }

		private Keys[] _allKeys;
		private bool[] _pressedKeys = new bool[256];

		public KeyInputHandler()
		{
			_allKeys = (Keys[])Enum.GetValues(typeof(Keys));
		}

		public void Update(GameTime time)
		{
			KeyboardState keyboardState = Keyboard.GetState();

			foreach (Keys key in _allKeys)
			{
				bool isKeyDown = keyboardState.IsKeyDown(key);

				if (isKeyDown != _pressedKeys[(int)key])
				{
					HandleKeyStateChanged(key, isKeyDown, keyboardState, time);
				}
			}
		}

		public void HandleKeyStateChanged(Keys key, bool isKeyDown, KeyboardState keyboardState, GameTime time)
		{
			_pressedKeys[(int)key] = isKeyDown;

			if (isKeyDown)
			{
				if (Listener != null)
				{
					bool shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
					bool capsLock = keyboardState.CapsLock;
					Listener.OnKeyDown(new KeyDownEventArgs(time, key, shift, capsLock));
				}
			}
			else
			{
				if (Listener != null)
					Listener.OnKeyUp(new KeyUpEventArgs(time, key));
			}
		}
	}
}
