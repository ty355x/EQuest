using System;
using Microsoft.Xna.Framework;

using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones.MobAI
{
	public class IdleAI : AIBase
	{
		private const int WanderMinSecs = 15;
		private const int WanderMaxSecs = 60;

		private const int InitialWanderMinSecs = 5;
		private const int InitialWanderMaxSecs = 15;

		private const int WanderRadius = 100;

		private long _nextCheckAggroTime = -1;
		private const int CheckAggroIntervalMs = 1500;

		private long _nextWalkTime = -1;

		public override MobAIType Type { get { return MobAIType.Idle; } }

		public IdleAI(Mob mob, Zone zone) : base(mob, zone)
		{
		}

		public override void Update(long time)
		{
			if (_mob.IsAggro)
				CheckAggro(time);

			if (_mob.Wander)
				CheckWander(time);
		}

		public void ReturnToSpawn()
		{
			if (_mob.Position != _mob.SpawnPoint)
				_mob.Movement.MoveTo(_mob.SpawnPoint);
		}

		private void CheckAggro(long time)
		{
			if (!(_nextCheckAggroTime == -1 || time >= _nextCheckAggroTime))
				return;

			foreach (var target in _zone.Mobs)
			{
				if (target.IsClient && _mob.CalcDistance(target) <= _mob.AggroRadius)
				{
					_mob.OnAttacked(time, target);
					break;
				}
			}

			_nextCheckAggroTime = time + CheckAggroIntervalMs;	
		}

		private void CheckWander(long time)
		{
			// Check if the next walk timer has been initialized.
			if (_nextWalkTime == -1)
			{
				UpdateNextWanderTime(time);
				return;
			}

			// Otherwise, check if the walk timer has expired.
			if (time >= _nextWalkTime)
			{
				DoWander(time);
				return;
			}
		}

		private void DoWander(long time)
		{
			int deltaX = ZoneServer.Random.Next(-WanderRadius, WanderRadius);
			int deltaY = ZoneServer.Random.Next(-WanderRadius, WanderRadius);

			Vector3 newPos = new Vector3(_mob.SpawnPoint.X + deltaX, _mob.SpawnPoint.Y + deltaY, _mob.SpawnPoint.Z);
			_mob.Movement.MoveTo(newPos);

			UpdateNextWanderTime(time);
		}

		private void UpdateNextWanderTime(long time)
		{
			int wanderMin = _nextWalkTime > -1 ? WanderMinSecs * 1000 : InitialWanderMinSecs * 1000;
			int wanderMax = _nextWalkTime > -1 ? WanderMaxSecs * 1000 : InitialWanderMinSecs * 1000;
			_nextWalkTime = time + ZoneServer.Random.Next(wanderMin, wanderMax);
		}
	}
}
