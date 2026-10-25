using System;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender
{
	public class Texture : IDisposable
	{
		public TextureImage Image { get => GetTextureImage(); }

		public int FragId = 0;
		public string Name;
		public TextureImage[] Images = null;
		public bool IsTranslucent = false;
		private int _currentImageIndex = 0;

		public Texture(int fragId, string fragName, TextureImage[] images)
		{
			this.FragId = fragId;
			this.Images = images;
			Name = DetermineTextureName(fragName);
		}
		
		public void Next()
		{
			if (Images == null || Images.Length == 1)
				return;
			
			_currentImageIndex++;

			if (_currentImageIndex >= Images.Length)
				_currentImageIndex = 0;
		}

		public void Dispose()
		{
			if (Images != null)
			{
				foreach (TextureImage tex in Images)
				{
					if (tex != null)
						tex.Dispose();
				}
			}

			Images = null;
		}

		private string DetermineTextureName(string fragName)
		{
			if (string.IsNullOrEmpty(fragName))
				return "Unknown";
			else if (fragName.EndsWith("_MDF"))
				return fragName.Substring(0, fragName.Length - 4);
			else
				return fragName;
		}

		private TextureImage GetTextureImage()
		{
			if (Images == null || Images.Length == 0)
				return null;
			else
				return Images[_currentImageIndex];
		}
	}
}
