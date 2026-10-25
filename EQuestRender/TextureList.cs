using System;
using System.Collections.Generic;

namespace EQuestRender
{
	public class TextureList
	{
		public string Name { get; private set; }
		public int FragIndex { get; private set; }
		public Texture this[int i] { get => _textures[i]; }
		public int Length { get => _textures.Length; }
		
		private Texture[] _textures = null;
		private List<TextureList> _altTextureLists = new List<TextureList>();

		public TextureList(string name, int fragIndex, Texture[] textures)
		{
			this.Name = name;
			this.FragIndex = fragIndex;
			_textures = textures;
		}

		public void AddAltTextureList(Texture[] textures)
		{
			_altTextureLists.Add(new TextureList(Name, FragIndex, textures));
		}

		public TextureList GetAltTextureList(int index)
		{
			return index < _altTextureLists.Count ? _altTextureLists[index] : this;
		}
	}
}
