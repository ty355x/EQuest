using System;
using EQuestServer;

namespace EQuestServer.Zones.Mobs
{
	public class MobCombat
	{
		private Mob _mob;
		
		public MobCombat(Mob mob)
		{
			_mob = mob;
		}

		public void DoCombat(long time, Mob target)
		{
			if (_mob.IsDisabled || target.IsDisabled)
				return;

			// Check if the time has expired before the next attack.
			if (!(_mob.NextAttack == -1 || time >= _mob.NextAttack))
				return;

			// Calculate the next attack time.
			_mob.NextAttack = time + _mob.CombatDelay * 100;

			// Make sure the attacker isn't attacking itself.
			if (_mob == target)
			{
				if (_mob.IsClient)
					_mob.Zone.Server.SendMessageToClient(_mob.Client, "You can't attack yourself, try picking another target");

				return;
			}

			// Make sure we can the target.
			if (_mob.CalcDistance(target) >= _mob.CanHitRadius)
			{
				if (_mob.IsClient)
					_mob.Client.SendMessage("You are too far away to hit your target, get closer!");

				return;
			}

			// Check whether there was a hit.
			bool didHit = ZoneServer.Random.Next(0, 100) > 25;

			if (didHit)
			{
				int damage = ZoneServer.Random.Next(_mob.MinDamage, _mob.MaxDamage);
				_mob.Zone.Combat.SendHit(_mob, target, damage);
				target.ChangeHitPoints(-damage, _mob);
			}
			else
			{
				// Update clients with a missed message.
				_mob.Zone.Combat.SendMissed(_mob, target);
			}

			// Notify the target that they were attacked.
			target.OnAttacked(time, _mob);
		}
	}
}
