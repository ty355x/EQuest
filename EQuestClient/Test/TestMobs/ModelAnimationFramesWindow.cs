using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using EQuestClient.AssetManagement;
using EQuestClient.UI;
using EQuestClient.UI.Controls;
using EQuestRender;

namespace EQuestClient.Test.TestMobs
{
	public class ModelAnimationFramesWindow : Window
	{
		public ModelAnimationFramesWindow()
		{
		}

		public void SetFrameCount(int count)
		{
			ClearChilden();
			int buttonPosX = 5;
			const int ButtonWidth = 40;
			
			for (int i = 0; i < count; i++)
			{
				var btn = new Button() { Text = i.ToString(), X = buttonPosX, Y = 5, Width = ButtonWidth, Height = 40 };
				AddChild(btn);
				buttonPosX += ButtonWidth + 5;
			}

			HighlightFrameButton(0);
		}

		public void HighlightFrameButton(int index)
		{
			for (int i = 0; i < _children.Count; i++)
			{
				var button = _children[i] as Button;
				button.BackgroundTint = i == index ? new Color(0, 0, 0, 0.5f) : Viewport.DefaultBackgroundTint;
			}
		}
		
		public override void Initialize()
		{
		}
	}
}
