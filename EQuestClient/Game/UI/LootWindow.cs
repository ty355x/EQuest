using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;
using EQuestClient.UI;
using EQuestClient.UI.Controls;

namespace EQuestClient.Game.UI
{
	public class LootWindow : Window
	{
		public const int DefaultWidth = 213;
		public const int DefaultHeight = 158;
		private const int ButtonCount = 15;

		private Label _corpseLabel;
		private Icon[] _buttons = new Icon[ButtonCount];
		private GameApp _game;
		private GameScene _scene;

		public LootWindow(GameApp game, GameScene scene)
		{
			_game = game;
			_scene = scene;
		}

		public override void Draw(Render render, GameTime time)
		{
			Update();
			base.Draw(render, time);
		}

		public override void Initialize()
		{
			_corpseLabel = new Label();
			_corpseLabel.Bounds = new Rectangle(4, 4, Bounds.Width, 10);
			_corpseLabel.TextColor = Color.White;
			AddChild(_corpseLabel);

			int buttonIndex = 0;

			for (int y = 0; y < 3; y++)
			{
				for (int x = 0; x < 5; x++)
				{
					var button = new Icon()
					{
						Bounds = new Rectangle(x * 41 + 4, y * 41 + 30, 40, 40),
						Background = _scene.Assets.UI.ItemSlot,
						Data = buttonIndex,
					};

					button.Click += OnLootButtonClick;
					_buttons[buttonIndex++] = button;
					AddChild(button);
				}
			}
		}

		private void Update()
		{
			Player player = _scene.Player;

			if (player == null)
				return;

			_corpseLabel.Text = player.Looting.Mob != null ? player.Looting.Mob.Name : null;
			
			for (int i = 0; i < 15; i++)
			{
				var lootEntry = i < _scene.Player.Looting.Loot.Count ? _scene.Player.Looting.Loot[i] : null;
				
				if (lootEntry != null)
				{
					_buttons[i].Image = _scene.Assets.ItemIcons.GetIcon(lootEntry.Item.Icon);
					_buttons[i].Tooltip = lootEntry.Item.Name;
				}
				else
				{
					_buttons[i].Image = null;
					_buttons[i].Tooltip = null;
				}
			}
		}

		private void OnLootButtonClick(UIElement button, int x, int y)
		{
			_scene.Player.Looting.RequestPickupLoot((int)button.Data);
		}
	}
}
