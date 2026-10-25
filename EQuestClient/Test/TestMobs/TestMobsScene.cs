using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EQuestClient.AssetManagement;
using EQuestClient.Game;
using EQuestClient.Scenes;
using EQuestRender;
using EQuestRender.Shaders;
using Viewport = EQuestClient.UI.Viewport;

namespace EQuestClient.Test.TestMobs
{
	public class TestMobsScene : Scene
	{
		private GameApp _game;
		private Render _render;
		private AssetManager _assets;
		private Viewport _viewport;
		private TestMobsInputHandler _input;
		private WorldShader _shader;
		
		private Mob _mob = null;
		private bool _initializedModel = false;
		private List<MobModel> _models;
		private GameTime _currentTime;
		
		private ModelNameWindow _modelNameWindow;
		private ModelAnimationWindow _modelAnimationWindow;
		private ModelAnimationFramesWindow _modelAnimationFramesWindow;
		
		public TestMobsScene(GameApp game, Render render)
		{
			_game = game;
			_render = render;
			_assets = new AssetManager(_game);
			_viewport = new Viewport(new Rectangle(0, 0, _render.Width, _render.Height));
			_input = new TestMobsInputHandler(_game, this, _viewport);
			_shader = new WorldShader(_assets.LoadAsset<Effect>(@"Shaders\Test"));
		}

		public override void Initialize()
		{
			_input.Initialize();
			_assets.LoadGlobalAssets();
			_models = _assets.GlobalModels;

			_render.Shader = _shader;
			_render.Camera.Position = Vector3.Zero;
			_render.Camera.World = Matrix.CreateScale(100.0f);
			_render.UpdateCamera();

			MakeUI();
		}

		public override void Update(GameTime time)
		{
			_currentTime = time;
			
			if (!_initializedModel)
			{
				ChangeModel("HUM");
				_initializedModel = true;
			}
			
			_input.Update(time);

			//if (_mob != null)
			//	_mob.Update(time);
		}

		public override void Draw(GameTime time)
		{
			_render.StartScene(Color.CornflowerBlue);
			
			if (_mob != null)
				_mob.Draw(_render, time, null, false, false);

			var mouseState = Mouse.GetState();
			_render.StartDraw2D();
			_viewport.Draw(_render, time, mouseState.Position.X, mouseState.Position.X);

			if (_mob != null)
				PrintMobAnimationInfo();

			_render.EndDraw2D();
			_render.EndScene();
		}

		public void ChangeModel(string name)
		{
			var model = (from assetModel in _models where assetModel.Name == name select assetModel).FirstOrDefault();

			if (model != null)
				_mob = new Mob(0, null, EQuestLib.MobType.Monster, 1, 1, 0, model, new Vector3(0, -15.0f, -3.5f), 0, 0, 0, 90, null);
			else
				_mob = null;

			_modelNameWindow.HighlightModelButton(name);
			_modelAnimationWindow.ChangeMob(_mob);

			ChangeAnimation(0);
		}

		public void ChangeAnimation(int index)
		{
			if (_mob != null)
				_mob.Model.SetAnimation(index, true, _currentTime.TotalGameTime.TotalMilliseconds);
			
			_modelAnimationWindow.ChangeAnimation(index);
			_modelAnimationFramesWindow.SetFrameCount(_mob.Model.CurrentAnimation.Skeleton[0].Frames.Count);
		}

		public void ShowPreviousFrame()
		{
			if (_mob != null && _currentTime != null)
			{
				_mob.Model.PreviousFrame(_currentTime.TotalGameTime.TotalMilliseconds);
				_modelAnimationFramesWindow.HighlightFrameButton(_mob.Model.AnimationFrameIndex);
			}
		}

		public void ShowNextFrame()
		{
			if (_mob != null && _currentTime != null)
			{
				_mob.Model.NextFrame(_currentTime.TotalGameTime.TotalMilliseconds);
				_modelAnimationFramesWindow.HighlightFrameButton(_mob.Model.AnimationFrameIndex);
			}
		}

		public override void Dispose()
		{
			RenderUtil.DisposeIfNotNull(_assets);
			_assets = null;
		}

		private void PrintMobAnimationInfo()
		{
			int y = 5;
			const int ItemMargin = 20;

			PrintSkeletonPiece(_mob.Model.CurrentAnimation.Skeleton[0], null, ref y, ItemMargin);
		}

		private void PrintSkeletonPiece(SkeletonPiece piece, SkeletonPiece parent, ref int y, int margin)
		{
			int x = _render.Width - 350;
			
			_render.DrawText(_assets.UI.DefaultFont, piece.Name, new Vector2(x, y += margin), Color.Red);

			if (parent != null)
				_render.DrawText(_assets.UI.DefaultFont, "Parent: " + parent.Name, new Vector2(x, y += margin), Color.Pink);

			SkeletonPieceFrame frame = piece.Frames[_mob.Model.AnimationFrameIndex];
			
			_render.DrawText(_assets.UI.DefaultFont, "SFT: " + MakeVector3Text(frame.Shift), new Vector2(x, y += margin), Color.Yellow);
			_render.DrawText(_assets.UI.DefaultFont, "ROT: " + MakeQuaternionText(frame.Rotate), new Vector2(x, y += margin), Color.Yellow);

			foreach (int childIndex in piece.Children)
				PrintSkeletonPiece(_mob.Model.CurrentAnimation.Skeleton[childIndex], piece, ref y, margin);
		}

		private string MakeVector3Text(Vector3 v)
		{
			return string.Format("({0:0.00}, {1:0.00}, {2:0.00})", v.X, v.Y, v.Z);
		}

		private string MakeQuaternionText(Quaternion q)
		{
			return string.Format("(X: {0:0.00}, Y: {1:0.00}, Z: {2:0.00}, W: {3:0.00})", q.X, q.Y, q.Z, q.W);
		}

		private void MakeUI()
		{
			InitializeUIDefaults();
			
			_modelNameWindow = new ModelNameWindow(this, _assets, _models) { X = 5, Y = 5, Width = 100, Height = _render.Height - 65 };
			_viewport.AddWindow(_modelNameWindow);
			
			_modelAnimationWindow = new ModelAnimationWindow(this) { X = 110, Y = 5, Width = 190, Height = _render.Height - 65 };
			_viewport.AddWindow(_modelAnimationWindow);

			_modelAnimationFramesWindow = new ModelAnimationFramesWindow() { X = 5, Y = _render.Height - 55, Width = _render.Width - 10, Height = 50 };
			_viewport.AddWindow(_modelAnimationFramesWindow);

			_viewport.AddWindow(new UI.Window() { X = _render.Width - 355, Y = 5, Width = 350, Height = _render.Height - 10 });
		}

		private void InitializeUIDefaults()
		{
			Viewport.DefaultTextFont = _assets.UI.DefaultFont;
			Viewport.DefaultBackground = _assets.UI.WindowBackground;
			Viewport.DefaultBackgroundTint = new Color(90, 90, 90, 120);
			Viewport.DefaultTransBackground = _assets.UI.TransFullImage;
			Viewport.DefaultProgressBackground = _assets.UI.Progress;
			Viewport.TooltipFont = _assets.UI.DefaultFont;
			Viewport.TooltipBackground = _assets.UI.WindowBackground;
			Viewport.ToolTipBackgroundTint = new Color(220, 220, 220, 220);
		}
	}
}
