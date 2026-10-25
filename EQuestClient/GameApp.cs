using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestConfig;
using EQuestRender;
using EQuestClient.Game;
using EQuestClient.Network;
using EQuestClient.Scenes;
using EQuestClient.VR;

namespace EQuestClient
{
	public class GameApp : Microsoft.Xna.Framework.Game
	{
		private const float FramerateCap = 333;
		
		public static string GameName = "EQuest";
		public static StartScenes StartScene = StartScenes.StartGame;
		public static bool Verbose = false;
		public static VRService VRService;

		private Render _render;
		private Texture2D _defaultTexture;
		private Scene _scene;
		private Effect _worldEffect;
		
		public GameScene GameScene { get; private set; }
		public Server Server { get; private set; }
		public Render Render { get { return _render; } }
		public GameConfig Config { get; private set; }
		public PlayerLogin Login { get; private set; }
		public string AssetsPath { get; private set; }
		public bool UseVR { get; private set; }

		public GameApp(PlayerLogin login, GameConfig config, string assetsPath, bool useVR)
		{
			Login = login;
			Config = config;
			AssetsPath = assetsPath;
			UseVR = useVR;
			
			Window.Title = GameName;
			Content.RootDirectory = "Content";
			IsFixedTimeStep = false;
			IsMouseVisible = true;
			// _render = new Render(this, Config.GetResolutionWidth(), Config.GetResolutionHeight(), Config.FullScreen);
			_render = new Render(this, 1920, 1080, false);
			Server = new Server();
		}

		public void ConnectionLost()
		{
			Log.Debug("Connection lost");
			ShowScene(null);
			Exit();
		}

		public void ShowScene(Scene newScene)
		{
			if (newScene != _scene)
			{
				RenderUtil.DisposeIfNotNull(_scene);
				_scene = newScene;

				if (_scene != null)
					_scene.Initialize();
			}
		}

		public void EnterZone(string zoneName, string longName, int playerMobId, Vector3 position, float heading)
		{
			Log.Debug("Entering zone " + zoneName);

			GameScene.Player.MobId = playerMobId;
			GameScene.Input.MouseSpeed = Config.MouseSpeed;
			GameScene.Player.Body.Position = position;
			GameScene.Player.Heading = heading;
			ShowScene(GameScene);
			GameScene.EnterZone(zoneName, longName);
		}

		protected override void Update(GameTime time)
		{
			if (_scene != null)
				_scene.Update(time);

			base.Update(time);
		}

		protected override void Draw(GameTime time)
		{
			if (_scene != null)
			{
				if (UseVR)
					VRService.DrawScene(_scene, time);
				else
					_scene.Draw(time);
			}

			base.Draw(time);
		}

		protected override void Initialize()
		{
			_render.Init();
			
			if (UseVR)
			{
				VRService = new VRService(_render);
				VRService.Init();
			}
			
			base.Initialize();
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			if (UseVR)
				VRService.SetupRender(Render.Graphics);
			
			_worldEffect = Content.Load<Effect>("Shaders\\DefaultWorld");
			_defaultTexture = Content.Load<Texture2D>("Textures\\TexDefault");
			_render.DefaultTexture = _defaultTexture;
			// _render.SetEffect(_worldEffect);

			GameScene = new GameScene(this, Server, _render);

			if (GameApp.StartScene == StartScenes.StartGame)
				ShowScene(new TitleScene(this, _render, AssetsPath));
			else if (GameApp.StartScene == StartScenes.TestMobsScene)
				ShowScene(new Test.TestMobs.TestMobsScene(this, _render));
			else
				throw new Exception("Unable to determine startup scene");
		}

		protected override void UnloadContent()
		{
			RenderUtil.DisposeIfNotNull(_worldEffect);
			_worldEffect = null;
			
			RenderUtil.DisposeIfNotNull(_scene);
			_scene = null;
			
			RenderUtil.DisposeIfNotNull(_render);
			_render = null;
			
			RenderUtil.DisposeIfNotNull(_defaultTexture);
			_defaultTexture = null;

			if (UseVR)
				VRService.Dispose();
			
			base.UnloadContent();
		}
	}
}
