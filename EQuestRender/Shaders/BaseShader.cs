using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender.Shaders
{
	public abstract class BaseShader : IDisposable	
	{
		public Effect Effect { get; set; }

		public virtual Matrix World { get; set; }
		public virtual Matrix View { get; set; }
		public virtual Matrix Projection { get; set; }
		public virtual bool FogEnabled { get; set; }
		public virtual Color FogColor { get; set; }
		public virtual float AlphaTestThreshold { get; set; }

		public BaseShader(Effect effect)
		{
			Effect = effect;
		}

		public virtual void ApplyChanges()
		{
			Effect.CurrentTechnique.Passes[0].Apply();
		}

		public virtual void SetTexture(Texture2D texture)
		{
		}

		public virtual void ApplyFog(float fogStart, float fogEnd)
		{
		}

		public virtual void RecalculateFog()
		{
		}

		public void Dispose()
		{
			if (Effect != null)
			{
				Effect.Dispose();
				Effect = null;
			}
		}
	}
}
