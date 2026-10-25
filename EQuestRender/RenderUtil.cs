using System;
using System.Collections.Generic;

namespace EQuestRender
{
	public static class RenderUtil
	{
		public static void DisposeIfNotNull(IDisposable disposable)
		{
			if (disposable != null)
				disposable.Dispose();
		}

		public static void Dispose(IEnumerable<IDisposable> list)
		{
			if (list != null)
			{
				foreach (var disposable in list)
				{
					if (disposable != null)
						disposable.Dispose();
				}
			}
		}
	}
}
