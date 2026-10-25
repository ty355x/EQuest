using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestLib.Data;
using EQuestRender;
using EQuestClient.Network;

namespace EQuestClient.Game
{
	public class GamePacketHandler : PacketHandlerBase
	{
		private GameApp _game;
		private GameScene _scene;
		private EnterZonePacketHandler _enterZoneHandler;

		public GamePacketHandler(GameApp game, GameScene scene, Server server) : base (server)
		{
			_game = game;
			_scene = scene;
			_enterZoneHandler = new EnterZonePacketHandler(_game);
		}

		protected override void HandleCommand(ServerCommand command, PacketIn packet)
		{
			if (command == ServerCommand.SpawnMob)
				HandleSpawnMob(packet);
			else if (command == ServerCommand.RemoveMob)
				HandleRemoveMob(packet);
			else if (command == ServerCommand.UpdateMobMovement)
				HandleUpdateMobMovement(packet);
			else if (command == ServerCommand.Message)
				HandleMessage(packet);
			else if (command == ServerCommand.CombatUpdate)
				_scene.Combat.HandleCombatUpdatePacket(packet);
			else if (command == ServerCommand.UpdateMobStatus)
				HandleUpdateMobStatus(packet);
			else if (command == ServerCommand.EnterZone)
				HandleEnterZone(packet);
			else if (command == ServerCommand.UpdateKnownItems)
				HandleUpdateKnownItems(packet);
			else if (command == ServerCommand.LootList)
				HandleLootList(packet);
			else if (command == ServerCommand.PickupLootItem)
				HandlePickupLootItem(packet);
			else
				Log.Debug("Received unrecognized packet " + command);
		}

		private void HandleSpawnMob(PacketIn packet)
		{
			// Get the mob data.
			int mobId = packet.ReadInt();
			string mobName = packet.ReadString();
			MobType mobType = (MobType)packet.ReadInt();
			int mobTexture = packet.ReadInt();
			float mobSize = packet.ReadFloat();
			int mobHitpoints = packet.ReadInt();
			int mobMaxHitpoints = packet.ReadInt();
			int mobHitpointPercent = packet.ReadInt();
			int mobRace = packet.ReadInt();
			int mobGender = packet.ReadInt();
			Vector3 mobPosition = packet.ReadVector3();
			float mobHeading = packet.ReadFloat();

			// Create the mob.
			ModelInfo mobModelInfo = GameData.Races.FindModelInfo(mobRace, mobGender);
			MobModel mobModel = _scene.Assets.FindMobModel(mobModelInfo.Name);

			//ModelInfo mobModelInfo = GameData.Races.FindModelInfo(24, 0);
			//MobModel mobModel = _scene.Assets.FindMobModel("FIS");

			Mob mob = new Mob(mobId, mobName, mobType, mobTexture, mobModelInfo.Size, mobSize, mobModel, mobPosition, mobHitpoints, 
				mobMaxHitpoints, mobHitpointPercent, mobHeading, _scene);
			mob.Race = mobRace;
			mob.Gender = mobGender;

			// Check if the mob is the current player.
			if (mobId == _scene.Player.MobId)
			{
				mob.IsCurrentPlayer = true;
				_scene.Player.Mob = mob;
				_scene.Player.Body.Position = mob.Position;
			}

			// Add the mob to the zone.
			_scene.Zone.AddMob(mob);
		}

		private void HandleRemoveMob(PacketIn packet)
		{
			int mobId = packet.ReadInt();
			_scene.Zone.RemoveMob(mobId);
		}

		private void HandleUpdateMobMovement(PacketIn packet)
		{
			int mobId = packet.ReadInt();
			Vector3 mobPosition = packet.ReadVector3();
			float mobSpeed = packet.ReadFloat();
			float mobHeading = packet.ReadFloat();
			int mobRotateDirection = packet.ReadInt();

			if (mobId == _scene.Player.MobId)
			{
				_scene.Player.Movement.StopMovement();
				_scene.Player.UpdatePositionFromServer(mobPosition, mobHeading);
			}
			else
			{
				_scene.Zone.UpdateMobMovement(mobId, mobPosition, mobSpeed, mobHeading, mobRotateDirection);
			}
		}

		private void HandleMessage(PacketIn packet)
		{
			string message = packet.ReadString();
			_scene.UI.Write(message);
		}

		private void HandleUpdateMobStatus(PacketIn packet)
		{
			int mobId = packet.ReadInt();
			int mobHitPoints = packet.ReadInt();
			int mobMaxHitPoints = packet.ReadInt();
			int mobHitPointPercent = packet.ReadInt();

			Mob mob = _scene.Zone.FindMob(mobId);

			if (mob == null)
				return;

			mob.HitPoints = mobHitPoints;
			mob.MaxHitPoints = mobMaxHitPoints;
			mob.HitpointPercent = mobHitPointPercent;
		}

		private void HandleEnterZone(PacketIn packet)
		{
			_enterZoneHandler.HandleEnterZone(packet);
		}

		private void HandleUpdateKnownItems(PacketIn packet)
		{
			_enterZoneHandler.HandleUpdateKnownItems(packet);
		}

		private void HandleLootList(PacketIn packet)
		{
			int mobId = packet.ReadInt();
			int cash = packet.ReadInt();
			int numItems = packet.ReadInt();
			var loot = new List<LootItem>();

			for (int i = 0; i < numItems; i++)
			{
				var index = packet.ReadInt();
				var itemId = packet.ReadInt();
				loot.Add(new LootItem(index, _scene.KnownItems.FindItem(itemId)));
			}

			_scene.Player.Looting.StartLooting(mobId, cash, loot);
		}

		private void HandlePickupLootItem(PacketIn packet)
		{
			int corpseId = packet.ReadInt();
			int itemIndex = packet.ReadInt();
			int itemId = packet.ReadInt();

			_scene.Player.Looting.PickupItem(corpseId, itemIndex, itemId);
		}
	}
}
