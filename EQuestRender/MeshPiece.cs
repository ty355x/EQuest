using System;

namespace EQuestRender
{
	public struct MeshPiece
	{
		public int SkelPieceIndex;
		public short VertexIndex;
		public short VertexCount;

		public MeshPiece(short index, short vertexIndex, short vertexCount)
		{
			this.SkelPieceIndex = index;
			this.VertexIndex = vertexIndex;
			this.VertexCount = vertexCount;
		}
	}
}
