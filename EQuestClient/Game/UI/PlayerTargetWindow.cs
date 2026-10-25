using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;
using EQuestClient.UI;
using EQuestClient.UI.Controls;

namespace EQuestClient.Game.UI
{
	public class PlayerTargetWindow : Window
	{
		public const int DefaultWidth = 160;
		public const int DefaultHeight = 62;

		private Label _targetNameLabel;
		private ProgressBar _targetProgressBar;
		private GameScene _scene;

		private int _lastHitpoints = -1;

		public PlayerTargetWindow(GameScene scene)
		{
			_scene = scene;
		}
		
		public override void Draw(Render render, GameTime time)
		{
			Update();
			base.Draw(render, time);
		}

		public override void Initialize()
		{
			int y = EQuestClient.UI.Viewport.ItemMargin;

			// Add the title label.
			var titleLabel = new Label();
			titleLabel.Bounds = new Rectangle(4, y, Width - 8, EQuestClient.UI.Viewport.DefaultTextHeight);
			titleLabel.Text = "Target";
			titleLabel.TextAlign = HAlign.Center;
			AddChild(titleLabel);

			y += EQuestClient.UI.Viewport.DefaultTextHeight + EQuestClient.UI.Viewport.ItemMargin;

			// Add the target name label.
			_targetNameLabel = new Label();
			_targetNameLabel.Bounds = new Rectangle(EQuestClient.UI.Viewport.ItemMargin, y, Width - EQuestClient.UI.Viewport.ItemMargin * 2,
				EQuestClient.UI.Viewport.DefaultTextHeight);
			AddChild(_targetNameLabel);

			y += EQuestClient.UI.Viewport.DefaultTextHeight + 2 * EQuestClient.UI.Viewport.ItemMargin;

			// Add the progress bar.
			_targetProgressBar = new ProgressBar();
			_targetProgressBar.Bounds = new Rectangle(4, y, Width - 8, EQuestClient.UI.Viewport.DefaultTextHeight);
			_targetProgressBar.TextAlign = HAlign.Center;
			_targetProgressBar.BackgroundTint = new Color(150, 0, 0, 100);

			AddChild(_targetProgressBar);
		}

		private void Update()
		{
			if (_scene.Player == null)
				return;
			
			Mob target = _scene.Player.Target;

			if (target != null)
			{
				_targetNameLabel.Text = target.Name;

				if (target.HitpointPercent != _lastHitpoints)
				{
					_targetProgressBar.Text = target.HitpointPercent + " %";
					_targetProgressBar.Value = target.HitpointPercent;
					_lastHitpoints = target.HitpointPercent;
				}
			}
			else
			{
				_targetNameLabel.Text = null;
				_targetProgressBar.Text = null;
				_targetProgressBar.Value = 0;
				_lastHitpoints = -1;
			}
		}
	}
}
