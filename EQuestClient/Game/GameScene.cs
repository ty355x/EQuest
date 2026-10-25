using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using EQuestRender;
using EQuestVR;
using EQuestClient.AssetManagement;
using EQuestClient.Game.Input;
using EQuestClient.Game.UI;
using EQuestClient.Scenes;
using EQuestClient.Network;
using EQuestClient.VR;

namespace EQuestClient.Game
{
	public class GameScene : Scene
	{
		private GameApp _game = null;
		private GameSceneUI _ui;
		private CombatHandler _combat;
		private bool _noClip = false;
		private GameNetwork _network;
		private Color _skyColor = new Color(0, 10, 40);
		private bool _isInitialized = false;
		private GameSceneRender _sceneRender;
		private Zone _zone = null;

		protected Player _player;
		protected Render _render = null;
		protected AssetManager _assets;
		
		public AssetManager Assets { get { return _assets; } }
		public CombatHandler Combat { get { return _combat; } }
		public KnownItems KnownItems { get; set; }
		public GameNetwork Network { get { return _network; } }
		public bool NoClip { get { return _noClip; } set { _noClip = value; } }
		public Player Player { get { return _player; } }
		public Render Render { get { return _render; } }
		public bool RenderWireframe { get { return false; } }
		public GameSceneUI UI { get { return _ui; } }
		public GameSceneInputHandler Input { get; private set; }
		public Zone Zone { get { return _zone; } }

		public GameScene(GameApp game, Server server, Render render)
		{
			_game = game;
			_render = render;

			_assets = new AssetManager(_game);
			Input = new GameSceneInputHandler(_game, this);
			KnownItems = new KnownItems(this);
			_player = new Player(this);
			_combat = new CombatHandler(_game, this);
			_network = new GameNetwork(_game, this, server);
			_ui = new GameSceneUI(_game, this);
			_sceneRender = new GameSceneRender(_assets, _player, _render, _ui);
		}

		public override void Initialize()
		{
			if (!_isInitialized)
			{
				_assets.LoadGlobalAssets();
				_ui.Initialize(_render.Width, _render.Height);
				_isInitialized = true;
			}
		}

		public void ConnectionLost()
		{
			_game.ConnectionLost();
		}

		public override void Update(GameTime time)
		{
			if (!_isInitialized || _zone == null)
				return;

			if (_game.IsActive)
				Input.Update(time);

			_network.Update(time);

			if (_zone != null && !_player.IsZoning)
			{
				_player.Update(time, _zone, _noClip);
				_zone.Update(time, _render);
			}
		}

		public override void Draw(GameTime time)
		{
			if (!_isInitialized || _zone == null)
				return;

			Vector3 cameraPosition = _player.Camera.Position;

			_render.Camera.Position = cameraPosition;
			_render.Camera.Rotation = _player.Rotation;
			
			_render.StartScene(Color.DarkGray);
			_sceneRender.DrawScene(_zone, time);
			_render.EndScene();
		}

		public override void DrawVR(Eye eye, Matrix hmdMatrix, GameTime time)
		{
			if (!_isInitialized || _zone == null)
				return;

			// VRRenderMatrices matrices = GameApp.VRService.MakeMatricesForEye(eye, hmdMatrix, cameraPosition, _player.Rotation);
		}

		public void EnterZone(string zoneName, string longName)
		{
			if (_zone != null)
			{
				_zone.Dispose();
				_zone = null;
			}

			_assets.LoadZoneAssets(zoneName);
			_zone = new Zone(_game.AssetsPath, zoneName, this, _render);
			
			_player.Unfreeze();
			_player.IsZoning = false;
			_player.MovementActions.StartFalling(null);
			
			_ui.Write("You have entered " + longName + ".");
			_game.Server.Send(PacketMaker.MakeEnteredZone());

#if !DEBUG
			// MediaPlayer.Play(_assets.BackgroundMusic);
#endif
		}

		public override void Dispose()
		{
			RenderUtil.DisposeIfNotNull(_assets);
			_assets = null;

			RenderUtil.DisposeIfNotNull(_zone);
			_zone = null;
		}
	}
}
