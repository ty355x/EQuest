using System;
using System.Linq;
using System.Collections.Generic;

namespace EQuestRender.Animations
{
	public class AnimationDef
	{
		public string ModelName { get; private set; }
		public Dictionary<string, AnimationDefAnimation> Animations { get; private set; }

		public AnimationDef(string modelName)
		{
			this.ModelName = modelName;
			this.Animations = new Dictionary<string, AnimationDefAnimation>();
		}

		public AnimationDefAnimation FindAnimation(string animationName)
		{
			return Animations.ContainsKey(animationName) ? Animations[animationName] : null;
		}

		public void AssignAnimations(MobModel model)
		{
			foreach (var entry in Animations)
			{
				// Don't add the animation if it already exists.
				if (model.Animations.Where(a => a.Name == entry.Key).Count() > 0)
					continue;

				// Add the new animation.
				model.Animations.Add(MakeAnimation(entry.Key, entry.Value, model));
			}
		}

		private MobModelAnimation MakeAnimation(string animationName, AnimationDefAnimation animationDef, MobModel model)
		{
			var skeleton = new SkeletonPiece[model.Skeleton.Length];

			for (int i = 0; i < model.Skeleton.Length; i++)
			{
				// Create the skeleton piece.
				skeleton[i] = model.Skeleton[i].Copy();

				if (skeleton[i].Name == null)
					continue;

				string pieceName = AnimationHelper.ExtractPieceName(skeleton[i].Name);

				// Get the related animation piece.
				var animPiece = animationDef.Pieces
					.Where(p => p.PieceName != null && p.PieceName == pieceName)
					.FirstOrDefault();

				if (animPiece == null)
					continue;
				
				// Add the frames from the animation piece.
				foreach (var frame in animPiece.Frag.Frames)
					skeleton[i].Frames.Add(AnimationHelper.MakeSkelPieceFrame(frame));

				skeleton[i].Frames.RemoveAt(0);
			}

			AnimationHelper.TransformSkeleton(model.Name, skeleton);
			return new MobModelAnimation(animationName, skeleton);
		}
	}
}
