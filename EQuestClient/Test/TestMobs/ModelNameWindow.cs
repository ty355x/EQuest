using System;
using System.Collections.Generic;
using System.Linq;
using EQuestClient.AssetManagement;
using EQuestClient.UI;
using EQuestClient.UI.Controls;
using EQuestRender;

namespace EQuestClient.Test.TestMobs
{
	public class ModelNameWindow : Window
	{
		private TestMobsScene _scene;
		private AssetManager _assets;
		private List<MobModel> _models;
		
		public ModelNameWindow(TestMobsScene scene, AssetManager assets, List<MobModel> models)
		{
			_scene = scene;
			_assets = assets;
			_models = models;
		}

		public void HighlightModelButton(string name)
		{
			foreach (var child in _children)
			{
				var button = child as Button;
				button.BackgroundTint = button.Text == name ? new Microsoft.Xna.Framework.Color(0, 0, 0, 0.5f) : Viewport.DefaultBackgroundTint;
			}
		}
		
		public override void Initialize()
		{
			int buttonPosY = 10;
			var modelNames = from model in _models orderby model.Name select model.Name;

			foreach (string name in modelNames)
			{
				var btn = new Button() { Text = name, X = 10, Y = buttonPosY, Width = 80, Height = 30 };
				btn.Click += OnButtonClick;
				AddChild(btn);
				buttonPosY += 35;
			}
		}

		private void OnButtonClick(UIElement sender, int x, int y)
		{
			string modelName = ((Button)sender).Text;
			_scene.ChangeModel(modelName);
		}
	}
}
