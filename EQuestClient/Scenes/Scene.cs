using System;
using Microsoft.Xna.Framework;
using EQuestVR;

namespace EQuestClient.Scenes
{
	public abstract class Scene : IDisposable
	{
		public virtual void Initialize()
		{
		}

		public virtual void Update(GameTime time)
		{
		}

		public virtual void Draw(GameTime time)
		{
		}

		public virtual void DrawVR(Eye eye, Matrix hmdMatrix, GameTime time)
		{
		}

		public virtual void Dispose()
		{
		}
	}
}
