using System;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;
using EQuestClient.UI;

namespace EQuestClient.AssetManagement
{
	public class ItemIcons : IDisposable
	{
		private const int ItemCount = 1512;
		private Texture2D[] _textures;
		private Image[] _icons;
		private GameApp _game;

		public ItemIcons(GameApp game)
		{
			_game = game;
		}

		public void LoadIcons()
		{
			Dispose();
			_textures = new Texture2D[ItemCount];
			_icons = new Image[ItemCount];

			for (int i = 0; i < ItemCount; i++)
			{
				_textures[i] = _game.Content.Load<Texture2D>(string.Format("Items\\{0:0000}", i));
				_icons[i] = new Image(_textures[i]);
			}
		}

		public Image GetIcon(int index)
		{
			return index > 0 && index < ItemCount ? _icons[index] : _icons[0];
		}

		public void Dispose()
		{
			RenderUtil.Dispose(_textures);
			_textures = null;
			_icons = null;
		}
	}
}
