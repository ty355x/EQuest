using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public class OcclusionTracker
	{

		public bool IsOccluded { get; set; } = false;
		public bool IsComplete { get => _occlusionQuery.IsComplete; }
		private OcclusionQuery _occlusionQuery;
		private int _occlusionThreshold;
		private bool _occlusionResultPending = false;
		
		public OcclusionTracker(GraphicsDevice graphics, int occlusionThreshold)
		{
			_occlusionThreshold = occlusionThreshold;
			_occlusionQuery = new OcclusionQuery(graphics);
		}

		public void BeginQuery()
		{
			_occlusionResultPending = true;
			_occlusionQuery.Begin();
		}

		public void EndQuery()
		{
			_occlusionQuery.End();
		}

		public void ProcessResult()
		{
			if (!_occlusionResultPending)
				return;
			
			if (!_occlusionQuery.IsComplete)
				IsOccluded = false;
			else
				IsOccluded = _occlusionQuery.PixelCount <= _occlusionThreshold;

			_occlusionResultPending = false;
		}

		public void WaitForCompletion()
		{
			if (_occlusionResultPending)
			{
				while (!_occlusionQuery.IsComplete)
				{
				}
			}
		}
	}
}
