using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using S3DFileLib;
using EQuestRender;
using EQuestRender.Animations;
using EQuestRender.Readers;
using Path = System.IO.Path;

namespace EQuestClient.AssetManagement
{
	public class AssetManager : IDisposable
	{
		public UIAssets UI { get; private set; }
		public Song BackgroundMusic { get; private set; }
		public UnitCube Cube { get; private set; }
		public ItemIcons ItemIcons { get; private set; }

		private GameApp _game;
		private SkyBox _skybox;
		private List<MobModel> _globalMobModels = new List<MobModel>();
		private TextureListContainer _globalMobTextures;
		private List<MobModel> _zoneMobModels = new List<MobModel>();
		private TextureListContainer _zoneMobTextures;
		private AnimationAssets _animations;
		
		public SkyBox SkyBox
		{
			get { return _skybox; }
		}

		public List<MobModel> GlobalModels
		{
			get { return _globalMobModels; }
		}

		public List<MobModel> ZoneMobModels
		{
			get { return _zoneMobModels; }
		}

		public AssetManager(GameApp game)
		{
			_game = game;
			_animations = new AnimationAssets(this);
			UI = new UIAssets(_game);
			ItemIcons = new ItemIcons(_game);
		}

		public T LoadAsset<T>(string assetName)
		{
			return _game.Content.Load<T>(assetName);
		}

		public void LoadGlobalAssets()
		{
			UI.Load();
			ItemIcons.LoadIcons();
			
			LoadSkybox();
			LoadGlobalModels();
			// BackgroundMusic = _game.Content.Load<Song>("Music\\Kelethin");
			// _titleImage = _game.Content.Load<Texture2D>("Images\\EQuest");
			Cube = new UnitCube(_game.Render.Graphics);
		}

		public void LoadZoneAssets(string zoneName)
		{
			DisposeZoneAssets();
			_zoneMobModels = new List<MobModel>();
			LoadModels(zoneName + "_chr", zoneName + "_chr", false, ref _zoneMobModels, ref _zoneMobTextures);
		}

		public MobModel FindMobModel(string name)
		{
			var mobModel = FindMobModelInternal(name);

			if (mobModel == null)
			{
				Log.Debug("Unable to find mob model " + name + ", using default model");
				mobModel = FindMobModelInternal("HUM");

				if (mobModel == null)
					throw new Exception("Unable to find default mob model");
			}
			
			return mobModel.Copy(_game.Render.Graphics);
		}

		private MobModel FindMobModelInternal(string name)
		{
			foreach (var model in _globalMobModels)
			{
				if (model.Name == name)
					return model;
			}

			foreach (var model in _zoneMobModels)
			{
				if (model.Name == name)
					return model;
			}

			return null;
		}

		private void LoadSkybox()
		{
			RenderUtil.DisposeIfNotNull(_skybox);
			_skybox = null;

			string skyBoxFilePath = Path.Combine(_game.AssetsPath, "sky.s3d");
			_skybox = new SkyBox(skyBoxFilePath, _game.Render.GDM.GraphicsDevice);
			_skybox.SkyIndex = 1;
		}

		private void LoadGlobalModels()
		{
			LoadModels("global_chr1", "global_chr", true, ref _globalMobModels, ref _globalMobTextures);
		}

		private void LoadModels(string s3dName, string wldName, bool global, ref List<MobModel> models, ref TextureListContainer textures)
		{
			using (var s3d = S3DFile.Open(_game.AssetsPath + "\\" + s3dName + ".s3d"))
			using (var wld = s3d.GetWldFile(wldName + ".wld"))
			{
				// Load global or zone animations.
				if (global)
					_animations.LoadGlobalAnimations(wld);
				else
					_animations.LoadZoneAnimations(wld);

				// Read textures.
				TextureReader textureReader = new TextureReader(s3d, wld);
				textures = textureReader.ReadTextures(_game.Render.Graphics);

				// Read models.
				var modelReader = new MobModelReader(s3d, wld);
				models = modelReader.ReadMobModels(textures, _game.Render.Graphics);

				// Assign model animations.
				foreach (var model in models)
					_animations.AssignAnimations(model);

				// Clear zone animations if we loaded them.
				if (!global)
					_animations.ClearZoneAnimations();
			}
		}

		private void LoadEquipment()
		{
			// Load global_chr models.
			//ModelListLoader.LoadModels(gamePath, "gequip", "gequip", _render.GraphicsDeviceManager.GraphicsDevice,
			//	out _equipModels, out _equipTextureSets);

			// Create scene models.
			//_equip = new SceneModel[_equipModels.Length];

			//for (int i = 0; i < _equipModels.Length; i++)
			//{
			//	_equip[i] = new SceneModel(_equipModels[i]);
			//	_equip[i].Position = new Vector3(300, 0, i * 15f);
			//}
		}

		public void Dispose()
		{
			DisposeZoneAssets();
			DisposeGlobalAssets();
		}

		private void DisposeZoneAssets()
		{
			RenderUtil.Dispose(_zoneMobModels);
			_zoneMobModels = null;

			RenderUtil.DisposeIfNotNull(_zoneMobTextures);
			_zoneMobTextures = null;
		}

		private void DisposeGlobalAssets()
		{
			UI.Dispose();
			ItemIcons.Dispose();

			RenderUtil.DisposeIfNotNull(_skybox);
			_skybox = null;

			RenderUtil.Dispose(_globalMobModels);
			_globalMobModels = null;

			RenderUtil.DisposeIfNotNull(_globalMobTextures);
			_globalMobTextures = null;

			RenderUtil.DisposeIfNotNull(BackgroundMusic);
			BackgroundMusic = null;

			RenderUtil.DisposeIfNotNull(Cube);
			Cube = null;
		}
	}
}
