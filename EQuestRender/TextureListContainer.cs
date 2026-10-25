using System;
using Microsoft.Xna.Framework;
using EQuestRender.Util;

namespace EQuestRender
{
	public class TextureListContainer : IDisposable
	{
		public Texture[] Textures { get => _textures; }
		public TextureList[] TextureLists { get => _textureLists; }

		// The rate at which texture lists with multiple textures will change to a new texture.
		private const double TextureChangeIntervalMs = 450;

		private Texture[] _textures;
		private TextureList[] _textureLists;
		private Texture[] _animatedTextures;
		private double _nextTextureChange = -1;

		public TextureListContainer(Texture[] textures, TextureList[] textureLists)
		{
			_textures = textures;
			_textureLists = textureLists;

			// Count animated textures.
			int animCount = 0;

			foreach (var tex in _textures)
			{
				if (tex.Images.Length > 1)
					animCount++;
			}

			// Create animated textures list.
			if (animCount > 0)
			{
				int i = 0;
				_animatedTextures = new Texture[animCount];

				foreach (var tex in _textures)
				{
					if (tex.Images.Length > 1)
						_animatedTextures[i++] = tex; 
				}
			}

			var altTextureListMaker = new AltTextureListMaker();
			altTextureListMaker.MakeAltTextureLists(_textures, _textureLists);
		}

		public void Update(GameTime time)
		{
			if (_animatedTextures == null)
				return;

			if (_nextTextureChange == -1)
			{
				_nextTextureChange = time.TotalGameTime.TotalMilliseconds + TextureChangeIntervalMs;
				return;
			}

			if (time.TotalGameTime.TotalMilliseconds >= _nextTextureChange)
			{
				foreach (var tex in _animatedTextures)
				{
					if (tex != null)
						tex.Next();
				}

				_nextTextureChange = time.TotalGameTime.TotalMilliseconds + TextureChangeIntervalMs;
			}
		}

		public TextureList FindTextureList(int fragIndex)
		{
			foreach (var ts in _textureLists)
			{
				if (ts.FragIndex == fragIndex)
					return ts;
			}

			return null;
		}

		public void Dispose()
		{
			if (_textures != null)
			{
				foreach (var tex in _textures)
				{
					if (tex != null)
						tex.Dispose();
				}

				_textures = null;
			}

			_textureLists = null;
			_animatedTextures = null;
		}
	}
}
