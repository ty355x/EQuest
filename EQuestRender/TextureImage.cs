using System;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public enum TextureProperty
	{
		Normal,
		Masked,
		Transparent,
	}
	
	public class TextureImage
	{
		public Texture2D Image { get; private set; }
		public TextureProperty Properties { get; private set; }
		public bool IsMaskedOrTransparent { get => Properties != TextureProperty.Normal; }

		public TextureImage(Texture2D image, TextureProperty properties)
		{
			this.Image = image;
			this.Properties = properties;
		}

		public void Dispose()
		{
			RenderUtil.DisposeIfNotNull(Image);
			Image = null;
		}
	}
}
