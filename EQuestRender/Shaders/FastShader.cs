using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender.Shaders
{
	public class FastShader : BaseShader	
	{
		public override Matrix World { get => _world;  set { _world = value; _worldEffect.SetValue(value); } }
		public override Matrix View { set { _view = value; _viewEffect.SetValue(value); } }
		public override Matrix Projection { set => _projectionEffect.SetValue(value); }
		public Color SurfaceColor { set => _surfaceColorEffect.SetValue(value.ToVector4()); }

		private Matrix _world;
		private Matrix _view;
		
		private EffectParameter _worldEffect;
		private EffectParameter _viewEffect;
		private EffectParameter _projectionEffect;
		private EffectParameter _surfaceColorEffect;
		
		public FastShader(Effect effect) : base(effect)
		{
			_worldEffect = Effect.Parameters["World"];
			_viewEffect = Effect.Parameters["View"];
			_projectionEffect = Effect.Parameters["Projection"];
			_surfaceColorEffect = Effect.Parameters["SurfaceColor"];
		}
	}
}
