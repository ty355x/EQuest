using System;
using System.Collections.Generic;

namespace EQuestRender
{
	public class SkeletonPiece
	{
		public string Name;
		public List<SkeletonPieceFrame> Frames = new List<SkeletonPieceFrame>();
		public int[] Children;
		public int VertIndex;
		public int VertCount;
		public MobMesh Mesh;

		public SkeletonPiece Copy()
		{
			var piece = new SkeletonPiece();
			piece.Name = Name;
			piece.Children = Children;
			piece.VertIndex = VertIndex;
			piece.VertCount = VertCount;
			piece.Mesh = Mesh;

			piece.Frames = new List<SkeletonPieceFrame>();

			foreach (var frame in Frames)
				piece.Frames.Add(frame.Copy());

			return piece;
		}
	}
}
