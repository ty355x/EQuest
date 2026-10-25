using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EQuestClient.Framework
{
	public class MouseInputHandler
	{
		public IMouseInputListener Listener { get; set; }
		
		private MouseState _lastMouseState;
		
		public void Update(GameTime time)
		{
			MouseState state = Mouse.GetState();
			Point pos = state.Position;

			if (state.LeftButton != _lastMouseState.LeftButton)
			{
				if (state.LeftButton == ButtonState.Pressed)
					OnButtonPressed(time, MouseButtons.LeftButton, pos);
				else
					OnButtonReleased(time, MouseButtons.LeftButton, pos);
			}

			if (state.RightButton != _lastMouseState.RightButton)
			{
				if (state.RightButton == ButtonState.Pressed)
					OnButtonPressed(time, MouseButtons.RightButton, pos);
				else
					OnButtonReleased(time, MouseButtons.RightButton, pos);
			}

			if (state.MiddleButton != _lastMouseState.MiddleButton)
			{
				if (state.MiddleButton == ButtonState.Pressed)
					OnButtonPressed(time, MouseButtons.MiddleButton, pos);
				else
					OnButtonReleased(time, MouseButtons.MiddleButton, pos);
			}

			_lastMouseState = state;
		}

		private void OnButtonPressed(GameTime time, MouseButtons button, Point position)
		{
			if (Listener != null)
				Listener.OnMouseButtonPressed(new MouseEventArgs(time, button, position));
		}

		private void OnButtonReleased(GameTime time, MouseButtons button, Point position)
		{
			if (Listener != null)
				Listener.OnMouseButtonReleased(new MouseEventArgs(time, button, position));
		}
	}
}
