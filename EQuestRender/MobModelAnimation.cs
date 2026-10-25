using System;

namespace EQuestRender
{
	public class MobModelAnimation
	{
		public string Name { get; private set; }
		public SkeletonPiece[] Skeleton { get; private set; }

		public MobModelAnimation(string name)
		{
			Name = name;
			Skeleton = new SkeletonPiece[0];
		}

		public MobModelAnimation(string name, SkeletonPiece[] skeleton)
		{
			Name = name;
			Skeleton = skeleton;
		}
	}
}
