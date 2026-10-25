using System;
using System.Diagnostics;

namespace EQuestServer
{
	public class Timer
	{
		private long _startTime;
		private Stopwatch _watch = new Stopwatch();

		public void Start()
		{
			_startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			_watch.Start();
		}

		public void Stop()
		{
			_watch.Stop();
		}

		public long CurrentTime
		{
			get { return _startTime + _watch.ElapsedMilliseconds; }
		}
	}
}
