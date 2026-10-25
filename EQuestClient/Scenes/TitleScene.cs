using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using S3DFileLib;
using EQuestRender;
using EQuestRender.Readers;
using EQuestClient.Network;
using EQuestVR;

namespace EQuestClient.Scenes
{
	class TitleScene : Scene
	{
		private GameApp _game;
		private Render _render;
		private Texture2D _titleImage;
		private SpriteFont _titleFont;
		private StartupNetwork _startupNetwork;
		private bool _hasConnected = false;

		public TitleScene(GameApp game, Render render, string gamePath)
		{
			_game = game;
			_render = render;
			LoadContent(gamePath);
			_startupNetwork = new StartupNetwork(_game, _game.Server);
		}

		public override void Draw(GameTime time)
		{
			DrawScene();
		}

		public override void DrawVR(Eye eye, Matrix hmdMatrix, GameTime time)
		{
			DrawScene();
		}

		public override void Update(GameTime time)
		{
			if (!_hasConnected)
			{
				_hasConnected = true;
				_game.Server.Start(_game.Config.ServerHost, int.Parse(_game.Config.ServerPort));
				_startupNetwork = new StartupNetwork(_game, _game.Server);
			}

			if (_startupNetwork != null)
				_startupNetwork.Update();
		}

		public override void Dispose()
		{
			RenderUtil.DisposeIfNotNull(_titleImage);
			_titleImage = null;
		}

		public void LoadContent(string gamePath)
		{
			_titleImage = _game.Content.Load<Texture2D>(@"Images\EQuest");
			_titleFont = _game.Content.Load<SpriteFont>(@"Fonts\LoadFont");

			/*var s3dPath = System.IO.Path.Combine(gamePath + "\\S3D", "bmpwad.s3d");

			using (var s3d = new S3DFile(s3dPath))
			{
				var texReader = new TextureReader(s3d, null);
				_titleBg = texReader.ReadImage("logo03.bmp", _render.Graphics);
				_titleFont = _app.Content.Load<SpriteFont>(@"Fonts\LoadFont");
			}*/
		}

		private void DrawScene()
		{
			float ratio = ((float)_titleImage.Width) / _titleImage.Height;
			int imageWidth = (int)(_render.Width * 0.75f);
			int imageHeight = (int)(imageWidth / ratio);

			var pos = new Vector2(
				_render.Width / 2 - imageWidth / 2,
				_render.Height / 2 - imageHeight / 2);

			_render.StartDraw2D();
			_render.Graphics.Clear(Color.Black);
			_render.DrawImage(_titleImage, new Rectangle((int) pos.X, (int) pos.Y, imageWidth, imageHeight), Color.White);

			pos.Y += imageHeight + 25;

			string displayText = _startupNetwork.State == StartupNetworkState.Connecting ?
				"Connecting to login server..." :
				"Loading assets...";

			_render.DrawText(_titleFont, displayText, pos, Color.Yellow);
			_render.EndDraw2D();
		}
	}
}
