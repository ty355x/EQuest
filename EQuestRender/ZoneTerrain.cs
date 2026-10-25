using System;

namespace EQuestRender
{
	public class ZoneTerrain : IDisposable
	{
		public Mesh Mesh;
		public bool HasTransparentTextures;
		public bool InsideFrustrum;
		public OcclusionTracker Occlusion;
		public ZoneTerrain Next;

		public ZoneTerrain(Mesh mesh)
		{
			this.Mesh = mesh;
		}

		public void Dispose()
		{
			if (Mesh != null)
			{
				Mesh.Dispose();
				Mesh = null;
			}
		}
	}
}
