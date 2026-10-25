using System;

namespace EQuestLib
{
	public static class EQuestMath
	{
		public static float FindShortestRotateDistance(float start, float end)
		{
			bool flipped = false;

			if (end > start)
			{
				float temp = start;
				start = end;
				end = temp;
				flipped = true;
			}

			float rotateRightDistance = end - start;
			float rotateLeftDistance = start + 360 - end;

			float shortestDistance = rotateRightDistance <= rotateLeftDistance ?
				rotateRightDistance :
				-rotateLeftDistance;

			if (flipped)
				shortestDistance = -shortestDistance;

			return shortestDistance;
		}
	}
}
