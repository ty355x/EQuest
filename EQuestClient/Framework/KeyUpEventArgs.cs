using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EQuestClient.Framework
{
	public struct KeyUpEventArgs
	{
		public GameTime Time { get; private set; }
		public Keys Key { get; private set; }

		public KeyUpEventArgs(GameTime time, Keys key)
		{
			this.Time = time;
			this.Key = key;
		}
	}
}
