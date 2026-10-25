using System;
using EQuestLib;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones.Combat
{
	public class CombatHandler
	{
		private Zone _zone;

		public CombatHandler(Zone zone)
		{
			_zone = zone;
		}

		public void HandleClientCombatUpdatePacket(Client client, PacketIn packet)
		{
			if (client.State != ClientState.EnteredZone)
				return;

			CombatUpdate update = (CombatUpdate)packet.ReadInt();

			if (update == CombatUpdate.StartAttack)
				HandleStartAttack(client, packet.ReadInt());
			else if (update == CombatUpdate.EndAttack)
				HandleStopAttack(client);
		}

		public void SendHit(Mob attacker, Mob target, int damage)
		{
			PacketOut packet = CombatPacketMaker.MakeCombatHit(attacker, target, damage);
			_zone.Server.SendPacketToAllActive(packet);
		}

		public void SendMissed(Mob attacker, Mob target)
		{
			PacketOut packet = CombatPacketMaker.MakeCombatMiss(attacker, target);
			_zone.Server.SendPacketToAllActive(packet);
		}

		private void HandleStartAttack(Client client, int mobId)
		{
			if (client.Mob.IsAttacking)
				return;

			Mob target = _zone.FindMob(mobId);

			if (target != null)
			{
				client.Mob.Target = target;
				client.Mob.IsAttacking = true;
				SendStartAttackCommand(client);
			}
		}

		private void HandleStopAttack(Client client)
		{
			client.Mob.IsAttacking = false;
			SendStopAttackCommand(client);
		}

		private void SendStartAttackCommand(Client client)
		{
			client.Send(CombatPacketMaker.MakeStartCombat());
		}

		private void SendStopAttackCommand(Client client)
		{
			client.Send(CombatPacketMaker.MakeStopCombat());
		}
	}
}
