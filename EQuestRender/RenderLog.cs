using System;

namespace EQuestRender
{
	public class RenderLog
	{
		public static void Debug(string text)
		{
			System.Diagnostics.Debug.WriteLine(text);
		}
	}
}
