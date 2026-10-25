using System;
using Microsoft.Xna.Framework;
using EQuestLib;

namespace EQuestServer.Zones
{
	public class ZonePacketHandler
	{
		private ZoneServer _server;

		public ZonePacketHandler(ZoneServer server)
		{
			_server = server;
		}

		public void HandlePacket(PacketIn packet)
		{
			Client client = (Client)packet.Sender;
			ClientCommand command = (ClientCommand)packet.ReadCommand();
			HandleCommand(client, command, packet);
		}

		private void HandleCommand(Client client, ClientCommand command, PacketIn packet)
		{
			if (command == ClientCommand.EnteredZone)
				HandleEnteredZone(client, packet);
			else if (command == ClientCommand.UpdateMovement)
				HandleUpdateMovement(client, packet);
			else if (command == ClientCommand.Message)
				HandleMessage(client, packet);
			else if (command == ClientCommand.CombatUpdate)
				HandleCombatUpdate(client, packet);
			else if (command == ClientCommand.ZoneLineRequest)
				HandleZoneLineRequest(client, packet);
			else if (command == ClientCommand.ZoneRequest)
				HandleZoneRequest(client, packet);
			else if (command == ClientCommand.StartLootCorpse)
				HandleLootCorpse(client, packet);
			else if (command == ClientCommand.LootItems)
				HandleLootItems(client, packet);
			else if (command == ClientCommand.PickupInventoryItem)
				HandlePickupInventoryItem(client, packet);
			else if (command == ClientCommand.DestroyItem)
				HandleDestroyItem(client, packet);
			else
				Log.Info("Got unknown client command: " + command);
		}

		private void HandleEnteredZone(Client client, PacketIn packet)
		{
			_server.Zone.HandleClientEnteredZone(client);
		}

		private void HandleUpdateMovement(Client client, PacketIn packet)
		{
			if (client.State != ClientState.EnteredZone)
				return;
			
			Vector3 position = packet.ReadVector3();
			float moveSpeed = packet.ReadFloat();
			float heading = packet.ReadFloat();
			int rotateDirection = packet.ReadInt();
			_server.Zone.UpdateClientMovement(client, position, moveSpeed, heading, rotateDirection);
		}

		private void HandleMessage(Client client, PacketIn packet)
		{
			string message = packet.ReadString();

			if (_server.ChatCommandHandler.IsChatCommand(message))
				_server.ChatCommandHandler.HandleChatCommand(client, message);
			else
				_server.SendClientMessageToAll(client, message);
		}

		private void HandleCombatUpdate(Client client, PacketIn packet)
		{
			_server.Zone.Combat.HandleClientCombatUpdatePacket(client, packet);
		}

		private void HandleZoneLineRequest(Client client, PacketIn packet)
		{
			int zoneLineId = packet.ReadInt();
			Vector3 clientPos = packet.ReadVector3();
			_server.HandleClientZoneLineRequest(client, zoneLineId, clientPos);
		}

		private void HandleZoneRequest(Client client, PacketIn packet)
		{
			int zoneId = packet.ReadInt();
			Vector3 clientPos = packet.ReadVector3();
			_server.HandleClientZoneRequest(client, zoneId, clientPos);
		}

		private void HandleLootCorpse(Client client, PacketIn packet)
		{
			int corpseId = packet.ReadInt();
			_server.Zone.StartLootCorpse(client, corpseId);
		}

		private void HandleLootItems(Client client, PacketIn packet)
		{
			int corpseId = packet.ReadInt();
			int itemIndex = packet.ReadInt();
			_server.Zone.LootCorpseItem(client.Mob, corpseId, itemIndex);
		}

		private void HandlePickupInventoryItem(Client client, PacketIn packet)
		{
			int slot = packet.ReadInt();
			client.Mob.PlayerInventory.PickupItem(slot);
		}

		private void HandleDestroyItem(Client client, PacketIn packet)
		{
			client.Mob.PlayerInventory.DestroyItem();
		}
	}
}
