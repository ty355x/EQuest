using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender.Shaders
{
	public class BasicEffectShader : BaseShader
	{
		public override Matrix World { set => _basicEffect.World = value; }
		public override Matrix View { set => _basicEffect.View = value; }
		public override Matrix Projection { set => _basicEffect.Projection = value; }

		private BasicEffect _basicEffect;
		
		public BasicEffectShader(GraphicsDevice graphics) : base(new BasicEffect(graphics))
		{
			_basicEffect = Effect as BasicEffect;
			_basicEffect.AmbientLightColor = new Vector3(1, 1, 1);
		}

		public override void SetTexture(Texture2D texture)
		{
			_basicEffect.Texture = texture;
			_basicEffect.CurrentTechnique.Passes[0].Apply();
		}
	}
}
