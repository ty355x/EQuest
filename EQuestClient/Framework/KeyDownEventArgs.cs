using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EQuestClient.Framework
{
	public struct KeyDownEventArgs
	{
		public GameTime Time { get; private set; }
		public Keys Key { get; private set; }
		public bool Shift { get; private set; }
		public bool CapsLock { get; private set; }

		public KeyDownEventArgs(GameTime time, Keys key, bool shift, bool capsLock)
		{
			this.Time = time;
			this.Key = key;
			this.Shift = shift;
			this.CapsLock = capsLock;
		}
	}
}
