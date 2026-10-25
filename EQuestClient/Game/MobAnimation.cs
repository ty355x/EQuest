using System;
using Microsoft.Xna.Framework;
using EQuestRender;

namespace EQuestClient.Game
{
	public class MobAnimation
	{
		public static bool CycleAnimations = false;
		private const string IdleAnimationName = "P01";
		private Mob _mob;
		private double _nextAnimationUpdate = 0;

		public MobAnimation(Mob mob)
		{
			_mob = mob;
			_mob.Model.SetAnimation("BASE", false, 0);
		}

		public void ShowWalk(int index, double time)
		{
			string requestedAnimation = string.Format("L{0:00}", index);

			if (_mob.Model.HasAnimation(requestedAnimation))
				_mob.Model.SetAnimation(requestedAnimation, false, time);
			else if (_mob.Model.HasAnimation("C01"))
				_mob.Model.SetAnimation("C01", false, time);
		}

		public void ShowCombat(int index, double time)
		{
			string requestedAnimation = string.Format("C{0:00}", index);

			if (_mob.Model.HasAnimation(requestedAnimation))
				_mob.Model.SetAnimation(requestedAnimation, false, time);
			else if (_mob.Model.HasAnimation("C05"))
				_mob.Model.SetAnimation("C05", false, time);
		}

		public void ShowDamage(double time)
		{
			_mob.Model.SetAnimation("D02", false, time);
		}

		public void ShowCorpse()
		{
			_mob.Model.SetAnimation("D05", false, 0);
		}

		public void ShowBestMovementAnimation()
		{
			char currentAnimationStartsWith = _mob.Model.CurrentAnimation.Name[0];

			if (!(currentAnimationStartsWith == 'B' || currentAnimationStartsWith == 'L' || currentAnimationStartsWith == 'P'))
				return;

			string bestAnimation = IdleAnimationName;

			if (_mob.IsCorpse)
				bestAnimation = "D05";
			else if (_mob.IsMoving)
				bestAnimation = "L01";

			if (_mob.Model.CurrentAnimation.Name != bestAnimation)
				_mob.Model.SetAnimation(bestAnimation, true, 0);
		}

		private void SetAnimation(string name, bool repeat, double time)
		{
			_mob.Model.SetAnimation(name, repeat, time);
		}

		public void Update(GameTime time)
		{
			double totalTime = time.TotalGameTime.TotalMilliseconds;
			
			if (_nextAnimationUpdate == 0)
			{
				_nextAnimationUpdate = totalTime + MobModel.AnimationFrameLength;
				return;
			}

			if (totalTime >= _nextAnimationUpdate)
			{
				if (CycleAnimations)
					ShowNextFrameOrAnimation(time);
				else
					ShowNextFrame(totalTime);

				_nextAnimationUpdate = totalTime + MobModel.AnimationFrameLength;
			}
		}

		public bool HasAnimation(string name)
		{
			return _mob.Model.HasAnimation(name);
		}

		public void ShowNextFrame(double totalTime)
		{
			if (!_mob.IsCorpse)
				ShowNextFrameForMob(totalTime);
			else
				ShowNextFrameForCorpse(totalTime);
		}

		private void ShowNextFrameForMob(double totalTime)
		{
			if (_mob.Model.AnimationFrameIndex >= _mob.Model.FrameCount - 1)
			{
				SetAnimation(IdleAnimationName, true, totalTime);
				ShowBestMovementAnimation();
			}
			else
			{
				_mob.Model.NextFrame(totalTime);
			}
		}

		private void ShowNextFrameForCorpse(double totalTime)
		{
			if (!(_mob.Model.AnimationFrameIndex >= _mob.Model.FrameCount - 1))
			{
				_mob.Model.NextFrame(totalTime);
			}
		}

		private void ShowNextFrameOrAnimation(GameTime time)
		{
			if (_mob.Model.AnimationFrameIndex + 1 >= _mob.Model.FrameCount)
				_mob.Model.SetAnimation(_mob.Model.AnimationIndex + 1, false, time.TotalGameTime.TotalMilliseconds);
			else
				_mob.Model.NextFrame(time.TotalGameTime.TotalMilliseconds);
		}
	}
}
