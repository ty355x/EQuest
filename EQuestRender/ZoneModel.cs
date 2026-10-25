using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace EQuestRender
{
	public class ZoneModel : IDisposable
	{
		private List<Mesh> _meshes;
		private TextureListContainer _textures;
		
		public List<Mesh> Meshes
		{
			get { return _meshes; }
		}

		public void Draw(GameTime time, Render render)
		{
			foreach (Mesh mesh in _meshes)
				render.DrawMesh(mesh, time);
		}

#if DEBUG2
		public void DrawCollision(GameTime time, Render render)
		{
			foreach (Mesh mesh in _meshes)
			{
				mesh.DrawCollisionPoly(render);
			}
		}
#endif

		public void Dispose()
		{
			RenderUtil.Dispose(_meshes);
			_meshes = null;

			RenderUtil.DisposeIfNotNull(_textures);
			_textures = null;
		}
	}
}
