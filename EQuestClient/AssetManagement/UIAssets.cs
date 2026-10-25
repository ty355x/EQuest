using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;
using EQuestClient.UI;

namespace EQuestClient.AssetManagement
{
	public class UIAssets
	{
		public Texture2D TransFull { get; private set; }
		public Image TransFullImage { get; private set; }
		public Image WindowBackground { get; private set; }
		public Image Progress { get; set; }
		public Image ItemSlot { get; private set; }
		public Image[] EquipSlots { get; private set; }

		public SpriteFont DefaultFont { get; private set; }
		public SpriteFont MobTitleFont { get; private set; }

		private Texture2D _windowBackground;
		private Texture2D _progress;
		private Texture2D _interface01;
		private Texture2D _interface02;
		private Texture2D _interface03;

		private GameApp _game;

		public UIAssets(GameApp game)
		{
			_game = game;
		}

		public void Load()
		{
			DefaultFont = _game.Content.Load<SpriteFont>(@"Fonts\UIFont");
			MobTitleFont = _game.Content.Load<SpriteFont>(@"Fonts\MobTitle");

			TransFull = _game.Content.Load<Texture2D>(@"UI\TransFull");
			_windowBackground = _game.Content.Load<Texture2D>(@"UI\WindowBackground");
			_progress = _game.Content.Load<Texture2D>(@"UI\Progress");
			_interface01 = _game.Content.Load<Texture2D>(@"UI\Interface01");
			_interface02 = _game.Content.Load<Texture2D>(@"UI\Interface02");
			_interface03 = _game.Content.Load<Texture2D>(@"UI\Interface03");

			TransFullImage = new Image(TransFull);
			WindowBackground = new Image(_windowBackground);
			Progress = new Image(_progress);
			ItemSlot = new Image(_interface01, new Rectangle(350, 82, 40, 40));
			
			MakeEquipSlots();
		}

		public void Dispose()
		{
			RenderUtil.DisposeIfNotNull(TransFull);
			TransFull = null;

			RenderUtil.DisposeIfNotNull(_windowBackground);
			_windowBackground = null;

			RenderUtil.DisposeIfNotNull(_progress);
			_progress = null;

			RenderUtil.DisposeIfNotNull(_interface01);
			_interface01 = null;

			RenderUtil.DisposeIfNotNull(_interface02);
			_interface02 = null;

			RenderUtil.DisposeIfNotNull(_interface03);
			_interface03 = null;

			TransFullImage = null;
			WindowBackground = null;
			Progress = null;
			ItemSlot = null;
			EquipSlots = null;
		}

		private void MakeEquipSlots()
		{
			EquipSlots = new Image[18];
			int slotIndex = 0;
			
			for (int j = 0; j < 3; j++)
			{
				for (int i = 0; i < 6; i++)
				{
					EquipSlots[slotIndex++] = new Image(_interface02, new Rectangle(i * 40, j * 40, 40, 40));
				}
			}
		}
	}
}
