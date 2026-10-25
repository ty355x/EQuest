using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;
using EQuestClient.UI;
using EQuestClient.UI.Controls;

namespace EQuestClient.Game.UI
{
	public class PlayerWindow : Window
	{
		public const int DefaultWidth = 160;
		public const int DefaultHeight = 70;

		private Color _normalBackgroundColor;
		private Color _combatBackgroundColor = Color.DarkRed;

		private Label _playerNameLabel;
		private ProgressBar _playerHitPointsBar;
		private ProgressBar _playerManaBar;

		private int _lastPlayerHP = -1;
		private GameScene _scene;

		public PlayerWindow(GameScene scene)
		{
			_scene = scene;
		}

		public override void Draw(Render render, GameTime time)
		{
			Update();
			BackgroundTint = _scene.Player.IsAttacking ? _combatBackgroundColor : _normalBackgroundColor;
			base.Draw(render, time);
		}

		public override void Initialize()
		{
			_normalBackgroundColor = EQuestClient.UI.Viewport.DefaultBackgroundTint;
			_combatBackgroundColor = new Color(100, 0, 0, 50);
			
			int y = 4;

			// Add the player name label
			_playerNameLabel = new Label();
			_playerNameLabel.Bounds = new Rectangle(4, y, DefaultWidth - 8, 16);
			AddChild(_playerNameLabel);

			y += 26;

			// Add the hitpoints bar.
			_playerHitPointsBar = new ProgressBar();
			_playerHitPointsBar.Bounds = new Rectangle(4, y, DefaultWidth - 8, 16);
			_playerHitPointsBar.TextAlign = HAlign.Center;
			_playerHitPointsBar.BackgroundTint = new Color(150, 0, 0, 100);
			AddChild(_playerHitPointsBar);

			y += 18;

			// Add the mana bar.
			_playerManaBar = new ProgressBar();
			_playerManaBar.Bounds = new Rectangle(4, y, DefaultWidth - 8, 16);
			_playerManaBar.Text = "100%";
			_playerManaBar.TextAlign = HAlign.Center;
			_playerManaBar.Value = 100;
			_playerManaBar.BackgroundTint = new Color(0, 0, 150, 100);
			AddChild(_playerManaBar);
		}

		private void Update()
		{
			Mob player = _scene.Player.Mob;

			if (player == null)
				return;

			_playerNameLabel.Text = player.Name;
			_playerHitPointsBar.Value = player.HitpointPercent;
			
			if (_lastPlayerHP != player.HitpointPercent)
			{
				_playerHitPointsBar.Text = player.HitpointPercent.ToString() + " %";
				_lastPlayerHP = player.HitpointPercent;
			}
		}
	}
}
