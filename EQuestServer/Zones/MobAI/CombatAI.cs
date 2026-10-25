using System;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones.MobAI
{
	public class CombatAI : AIBase
	{
		private const int CallForHelpIntervalMs = 2000;
		private long _nextCallForHelpTime = -1;
		public override MobAIType Type { get { return MobAIType.Combat; } }

		public CombatAI(Mob mob, Zone zone) : base (mob, zone)
		{
		}

		public void StartAttack(long time, Mob attacker)
		{
			if (_mob.AggroMessage != null && _mob.AggroMessage.Length > 0)
				_zone.Server.SendMessageToAll(_mob.Name + " says: " + _mob.AggroMessage);

			OnAttacked(time, attacker);
			CallForHelp(time);
			_mob.Movement.MoveNearTarget();
		}

		public void OnAttacked(long time, Mob attacker)
		{
			_mob.Target = attacker;
			_mob.IsAttacking = true;
		}

		public override void Update(long time)
		{
			// If we have lost aggro, go back to the idle ai.
			if (_mob.Target == null || !_mob.IsAttacking)
			{
				_mob.SetAI(new IdleAI(_mob, _zone));
				return;
			}

			// Check if the target has moved.
			if (_mob.Target.Position != _mob.LastTargetPosition)
				_mob.Movement.MoveNearTarget();
			
			// Check if it's time to call for help.
			if (_nextCallForHelpTime == -1 || time >= _nextCallForHelpTime)
			{
				CallForHelp(time);
				return;
			}
		}

		private void CallForHelp(long time)
		{
			foreach (var otherMob in _zone.Mobs)
				otherMob.CheckAssist(time, _mob);

			_nextCallForHelpTime = time + CallForHelpIntervalMs;
		}
	}
}
