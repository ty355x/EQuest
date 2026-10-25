using System;
using Microsoft.Xna.Framework;

namespace EQuestLib
{
	public static class Util
	{
		public static bool HasTimeElapsed(GameTime currentTime, double lastTimeMs, double elapsedTimeMs)
		{
			return lastTimeMs == -1 || 
				currentTime.TotalGameTime.TotalMilliseconds - lastTimeMs >= elapsedTimeMs;
		}
	}
}
