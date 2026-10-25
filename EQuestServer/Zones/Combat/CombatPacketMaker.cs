using System;

using EQuestLib;
using EQuestServer.Zones.Mobs;

namespace EQuestServer.Zones.Combat
{
	public static class CombatPacketMaker
	{
		public static PacketOut MakeStartCombat()
		{
			return PacketOut.MakePacket(ServerCommand.CombatUpdate)
				.WriteInt((int)ServerCombatUpdate.StartAttack)
				.End();
		}

		public static PacketOut MakeStopCombat()
		{
			return PacketOut.MakePacket(ServerCommand.CombatUpdate)
				.WriteInt((int)ServerCombatUpdate.EndAttack)
				.End();
		}

		public static PacketOut MakeCombatHit(Mob attacker, Mob target, int damage)
		{
			return PacketOut.MakePacket(ServerCommand.CombatUpdate)
				.WriteInt((int)ServerCombatUpdate.Hit)
				.WriteInt(attacker.Id)
				.WriteInt(target.Id)
				.WriteInt(damage)
				.End();
		}

		public static PacketOut MakeCombatMiss(Mob attacker, Mob target)
		{
			return PacketOut.MakePacket(ServerCommand.CombatUpdate)
				.WriteInt((int)ServerCombatUpdate.Missed)
				.WriteInt(attacker.Id)
				.WriteInt(target.Id)
				.End();
		}

		public static PacketOut MakeSlain(Mob corpse, Mob dead, Mob attacker)
		{
			int attackerId = attacker != null ? attacker.Id : -1;

			return PacketOut.MakePacket(ServerCommand.CombatUpdate)
				.WriteInt((int)ServerCombatUpdate.Slain)
				.WriteInt(corpse.Id)
				.WriteString(corpse.Name)
				.WriteInt(corpse.Race)
				.WriteInt(corpse.Gender)
				.WriteInt(corpse.TextureIndex)
				.WriteFloat(corpse.Size)
				.WriteVector3(corpse.Position)
				.WriteFloat(corpse.Heading)
				.WriteInt(dead.Id)
				.WriteString(dead.Name)
				.WriteInt(attackerId)
				.WriteString(attacker.Name)
				.End();
		}
	}
}
