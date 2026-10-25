using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender.Shaders
{
	public class WorldShader : BaseShader	
	{
		public override Matrix World { get => _world;  set { _world = value; _worldEffect.SetValue(value); } }
		public override Matrix View { set { _view = value; _viewEffect.SetValue(value); } }
		public override Matrix Projection { set => _projectionEffect.SetValue(value); }
		public override bool FogEnabled { set => _fogEnabledEffect.SetValue(value); }
		public override Color FogColor { set => _fogColorEffect.SetValue(value.ToVector4()); }
		public override float AlphaTestThreshold { get => _alphaTestThreshold; set { _alphaTestThreshold = value; _alphaTestEffect.SetValue(new Vector4(_alphaTestThreshold, 0, -1, 1)); } }

		private Matrix _world;
		private Matrix _view;
		private float _alphaTestThreshold;
		private float _fogStart;
		private float _fogEnd;

		private EffectParameter _textureEffect;
		private EffectParameter _worldEffect;
		private EffectParameter _viewEffect;
		private EffectParameter _projectionEffect;
		private EffectParameter _fogEnabledEffect;
		private EffectParameter _fogColorEffect;
		private EffectParameter _fogVectorEffect;
		private EffectParameter _alphaTestEffect;

		public WorldShader(Effect effect) : base(effect)
		{
			_textureEffect = Effect.Parameters["Texture"];
			_worldEffect = Effect.Parameters["World"];
			_viewEffect = Effect.Parameters["View"];
			_projectionEffect = Effect.Parameters["Projection"];
			_fogEnabledEffect = Effect.Parameters["FogEnabled"];
			_fogColorEffect = Effect.Parameters["FogColor"];
			_fogVectorEffect = Effect.Parameters["FogVector"];
			_alphaTestEffect = Effect.Parameters["AlphaTest"];
		}

		public override void ApplyChanges()
		{
			Effect.CurrentTechnique.Passes[0].Apply();
		}

		public override void SetTexture(Texture2D texture)
		{
			_textureEffect.SetValue(texture);
			Effect.CurrentTechnique.Passes[0].Apply();
		}

		public override void ApplyFog(float fogStart, float fogEnd)
		{
			_fogStart = fogStart;
			_fogEnd = fogEnd;
			RecalculateFog();
		}

		public override void RecalculateFog()
		{
			var scale = 1.0f / (_fogStart - _fogEnd);
			var worldView = _world * _view;

			var fogVector = new Vector4(worldView.M13 * scale, worldView.M23 * scale, worldView.M33 * scale,
				(worldView.M43 + _fogStart) * scale);

			_fogVectorEffect.SetValue(fogVector);
		}
	}
}
