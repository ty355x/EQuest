using System;
using System.Linq;
using System.Collections.Generic;
using S3DFileLib.WldFrags;

namespace EQuestRender.Animations
{
	public class AnimationDefAnimation
	{
		public string AnimationName { get; private set; }
		public List<AnimationDefPiece> Pieces { get; private set; }

		public AnimationDefAnimation(string animationName)
		{
			this.AnimationName = animationName;
			Pieces = new List<AnimationDefPiece>();
		}

		public AnimationDefPiece FindPiece(string name)
		{
			return Pieces.Where(p => p.PieceName == name).FirstOrDefault();
		}
	}
}
