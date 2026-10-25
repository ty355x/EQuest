using System;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestClient.Game;

namespace EQuestClient.Network
{
	public static class PacketMaker
	{
		public static PacketOut MakeLogin(string email, string password)
		{
			return PacketOut.MakePacket(ClientCommand.Login)
				.WriteString(email)
				.WriteString(password)
				.End();
		}

		public static PacketOut MakeEnteredZone()
		{
			return PacketOut.MakePacket(ClientCommand.EnteredZone).End();
		}

		public static PacketOut MakeMovementUpdate(Player player)
		{
			return PacketOut.MakePacket(ClientCommand.UpdateMovement)
				.WriteVector3(player.Body.Position)
				.WriteFloat(player.Movement.MoveSpeed)
				.WriteFloat(player.Heading)
				.WriteInt(player.RotateDirection)
				.End();
		}

		public static PacketOut MakeMessage(string message)
		{
			return PacketOut.MakePacket(ClientCommand.Message)
				.WriteString(message)
				.End();
		}

		public static PacketOut MakeCombatUpdate(Mob mob, CombatUpdate update)
		{
			if (update == CombatUpdate.StartAttack)
			{
				return PacketOut.MakePacket(ClientCommand.CombatUpdate)
					.WriteInt((int)CombatUpdate.StartAttack)
					.WriteInt(mob.ID)
					.End();
			}
			else if (update == CombatUpdate.EndAttack)
			{
				return PacketOut.MakePacket(ClientCommand.CombatUpdate)
					.WriteInt((int)CombatUpdate.EndAttack)
					.End();
			}
			else
			{
				return null;
			}
		}

		public static PacketOut MakeZoneLineRequest(int zoneLineId, Vector3 playerPos)
		{
			return PacketOut.MakePacket(ClientCommand.ZoneLineRequest)
				.WriteInt(zoneLineId)
				.WriteVector3(playerPos)
				.End();
		}

		public static PacketOut MakeZoneRequest(int zoneId, Vector3 playerPos)
		{
			return PacketOut.MakePacket(ClientCommand.ZoneRequest)
				.WriteInt(zoneId)
				.WriteVector3(playerPos)
				.End();
		}

		public static PacketOut MakeLootCorpse(int corpseId)
		{
			return PacketOut.MakePacket(ClientCommand.StartLootCorpse)
				.WriteInt(corpseId)
				.End();
		}

		public static PacketOut MakeLootItems(int corpseId, int itemIndex)
		{
			return PacketOut.MakePacket(ClientCommand.LootItems)
				.WriteInt(corpseId)
				.WriteInt(itemIndex)
				.End();
		}

		public static PacketOut MakePickupInventoryItem(int slot)
		{
			return PacketOut.MakePacket(ClientCommand.PickupInventoryItem)
				.WriteInt(slot)
				.End();
		}

		public static PacketOut MakeDestroyItem()
		{
			return PacketOut.MakePacket(ClientCommand.DestroyItem)
				.End();
		}
	}
}
