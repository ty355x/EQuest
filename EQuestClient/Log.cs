using System;
using Trace = System.Diagnostics.Trace;

namespace EQuestClient
{
	public class Log
	{
		public static void Debug(string text)
		{
			Trace.WriteLine(text);
		}
	}
}
