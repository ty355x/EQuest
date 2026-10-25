using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using EQuestRender;

namespace EQuestClient.Game
{
	public class MobBoundingBox : IDisposable
	{
		public BoundingBox Bounds;
		private Box _box = null;

#if DEBUG2
		private bool _graphicsNeedRefresh;
#endif
		
		public MobBoundingBox(GraphicsDevice graphics)
		{

#if DEBUG2
			_box = new Box(graphics);
#endif
		}

		public void Update(Mob mob)
		{
			float width = mob.Width / 2;
			
			Bounds.Min = new Vector3(mob.Position.X - width, mob.Position.Y - width, mob.Position.Z);
			Bounds.Max = new Vector3(mob.Position.X + width, mob.Position.Y + width, mob.Position.Z + mob.Height);
#if DEBUG2
			_graphicsNeedRefresh = true;
#endif
		}

		public void Draw(Render render)
		{
#if DEBUG2
			if (_graphicsNeedRefresh)
			{
				_box.RemakeCube(Bounds.Min, Bounds.Max, render.Graphics);
				_graphicsNeedRefresh = false;
			}
#endif
			render.DrawCube(_box, Vector3.Zero, 1);
		}

		public void Dispose()
		{
			RenderUtil.DisposeIfNotNull(_box);
			_box = null;
		}
	}
}
