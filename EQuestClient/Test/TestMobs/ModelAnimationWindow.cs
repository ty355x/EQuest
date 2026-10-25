using System;
using System.Linq;
using EQuestClient.AssetManagement;
using EQuestClient.Game;
using EQuestClient.UI;
using EQuestClient.UI.Controls;

namespace EQuestClient.Test.TestMobs
{
	public class ModelAnimationWindow : Window
	{
		private TestMobsScene _scene;
		private Mob _mob;
		
		public ModelAnimationWindow(TestMobsScene scene)
		{
			_scene = scene;
		}

		public void ChangeMob(Mob mob)
		{
			_mob = mob;
			ClearChilden();

			if (_mob == null)
				return;

			int buttonPosX = 10;
			int buttonPosY = 10;
			int i = 0;

			foreach (var anim in _mob.Model.Animations)
			{
				var btn = new Button() { Text = anim.Name, X = buttonPosX, Y = buttonPosY, Width = 80, Height = 30, Data = i++ };
				btn.Click += OnButtonClick;
				AddChild(btn);

				buttonPosY += 35;

				if (buttonPosY >= 1000)
				{
					buttonPosX += 90;
					buttonPosY = 10;
				}
			}
		}
		
		public override void Initialize()
		{
		}

		public void ChangeAnimation(int index)
		{
			for (int i = 0; i < _children.Count; i++)
			{
				var button = _children[i] as Button;
				button.BackgroundTint = i == index ? new Microsoft.Xna.Framework.Color(0, 0, 0, 0.5f) : Viewport.DefaultBackgroundTint;
			}
		}

		private void OnButtonClick(UIElement sender, int x, int y)
		{
			_scene.ChangeAnimation((int)((Button)sender).Data);
		}
	}
}
