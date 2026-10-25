using System;
using System.Collections.Generic;
using S3DFileLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender.Readers;

namespace EQuestRender
{
	public class SkyBox : IDisposable
	{
		public SkyBox(string s3dPath, GraphicsDevice graphics)
		{
			using (var s3d = S3DFile.Open(s3dPath))
			using (var wldNew = s3d.GetWldFile("sky.wld"))
			{
				ModelReader reader = new ModelReader();
				List<Mesh> models;
				reader.ReadModels(s3d, wldNew, graphics, out models, out _textures);

				foreach (var model in models)
				{
					if (model.Name.StartsWith("LAYER"))
						_models.Add(model);
				}

				if (_models.Count == 0)
					throw new Exception("Sky models not found in sky.wld");
			}
		}

		public void Dispose()
		{
			if (_models != null)
			{
				foreach (Mesh m in _models)
					m.Dispose();

				_models = null;
			}

			if (_textures != null)
			{
				_textures.Dispose();
				_textures = null;
			}

		}

		public int SkyIndex
		{
			get { return _skyIndex; }

			set
			{
				_skyIndex = value;

				if (_skyIndex < 0 || _skyIndex >= ModelCount)
					_skyIndex = _models.Count - 1;
			}
		}

		public Mesh CurrentSky
		{
			get { return _models[_skyIndex]; }
		}

		public int ModelCount
		{
			get { return _models.Count; }
		}

		private int _skyIndex = 0;
		private List<Mesh> _models = new List<Mesh>();
		private TextureListContainer _textures;
	}
}
