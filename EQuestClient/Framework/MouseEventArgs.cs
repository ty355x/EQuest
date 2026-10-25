using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EQuestClient.Framework
{
	public enum MouseButtons
	{
		LeftButton,
		MiddleButton,
		RightButton,
	}
		
	public struct MouseEventArgs
	{
		public GameTime Time { get; private set; }
		public MouseButtons Button { get; private set; }
		public Point Position { get; private set; }

		public MouseEventArgs(GameTime time, MouseButtons button, Point position)
		{
			this.Time = time;
			this.Button = button;
			this.Position = position;
		}
	}
}
