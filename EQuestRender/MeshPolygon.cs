using System;

namespace EQuestRender
{
	public class MeshPolygon
	{
		public short PolyCount;
		public short TextureIndex;

		public MeshPolygon(short polyCount, short textureIndex)
		{
			this.PolyCount = polyCount;
			this.TextureIndex = textureIndex;
		}
	}
}
