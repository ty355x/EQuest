using System;
using S3DFileLib.WldFrags;

namespace EQuestRender.Animations
{
	public class AnimationDefPiece
	{
		public string PieceName { get; private set; }
		public Frag12 Frag { get; private set; }

		public AnimationDefPiece(Frag12 frag)
		{
			this.Frag = frag;
			this.PieceName = AnimationHelper.ExtractPieceName(frag.FragName);
		}
	}
}
