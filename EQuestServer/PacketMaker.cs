using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestServer.Repositories;
using EQuestServer.Zones.Mobs;

namespace EQuestServer
{
	public static class PacketMaker
	{
		public static PacketOut MakeRequestLogin()
		{
			return PacketOut.MakePacket(ServerCommand.RequestLogin)
				.End();
		}
		
		public static PacketOut MakeEnterZone(string zoneName, string zoneLongName, Mob playerMob)
		{
			var packet = PacketOut.MakePacket(ServerCommand.EnterZone)
				.WriteString(zoneName)
				.WriteString(zoneLongName)
				.WriteInt(playerMob.Id)
				.WriteVector3(playerMob.Position)
				.WriteFloat(playerMob.Heading);

			if (playerMob.PlayerInventory == null)
				packet.WriteInt(0);
			else
				playerMob.PlayerInventory.WriteToPacket(packet);
			
			return packet.End();	
		}

		public static PacketOut MakeUpdateKnownItems(List<DbItem> items)
		{
			var packet = PacketOut.MakePacket(ServerCommand.UpdateKnownItems)
				.WriteInt(items.Count);

			foreach (var item in items)
			{
				packet.WriteInt(item.id)
				.WriteString(item.name)
				.WriteInt(item.icon - 500);
			}

			return packet.End();
		}

		public static PacketOut MakeSpawnMob(Mob mob)
		{
			return PacketOut.MakePacket(ServerCommand.SpawnMob)
				.WriteInt(mob.Id)
				.WriteString(mob.Name)
				.WriteInt((int)mob.Type)
				.WriteInt(mob.TextureIndex)
				.WriteFloat(mob.Size)
				.WriteInt(mob.HitPoints)
				.WriteInt(mob.MaxHitPoints)
				.WriteInt(mob.HitpointPercent)
				.WriteInt(mob.Race)
				.WriteInt(mob.Gender)
				.WriteVector3(mob.Position)
				.WriteFloat(mob.Heading)
				.WriteBool(mob.IsDisabled)
				.End();
		}

		public static PacketOut MakeRemoveMob(Mob mob)
		{
			return PacketOut.MakePacket(ServerCommand.RemoveMob)
				.WriteInt(mob.Id)
				.End();
		}

		public static PacketOut MakeMobMovementUpdate(Mob mob)
		{
			return PacketOut.MakePacket(ServerCommand.UpdateMobMovement)
				.WriteInt(mob.Id)
				.WriteVector3(mob.Position)
				.WriteFloat(mob.MoveSpeed)
				.WriteFloat(mob.Heading)
				.WriteFloat(mob.Movement.RotateDirection)
				.End();
		}

		public static PacketOut MakeMessage(string message)
		{
			return PacketOut.MakePacket(ServerCommand.Message)
				.WriteString(message)
				.End();
		}

		public static PacketOut MakeUpdateMobStats(Mob mob)
		{
			return PacketOut.MakePacket(ServerCommand.UpdateMobStatus)
				.WriteInt(mob.Id)
				.WriteInt(mob.IsClient ? -1 : mob.HitPoints)
				.WriteInt(mob.IsClient ? -1 : mob.MaxHitPoints)
				.WriteInt(mob.HitpointPercent)
				.End();
		}

		public static PacketOut MakeLootList(Mob mob)
		{
			int cash = !mob.Loot.IsLooted ? mob.Loot.Cash : -1;
			
			var packet = PacketOut.MakePacket(ServerCommand.LootList)
				.WriteInt(mob.Id)
				.WriteInt(cash)
				.WriteInt(mob.Loot.Entries.Count);

			foreach (var entry in mob.Loot.Entries)
			{
				packet.WriteInt(entry.Index)
					.WriteInt(entry.Item.id);
			}
			
			return packet.End();
		}

		public static PacketOut MakePickupLootItem(int corpseId, int itemIndex, int itemId)
		{
			return PacketOut.MakePacket(ServerCommand.PickupLootItem)
				.WriteInt(corpseId)
				.WriteInt(itemIndex)
				.WriteInt(itemId)
				.End();
		}
	}
}
