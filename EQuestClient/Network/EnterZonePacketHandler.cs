using System;
using Microsoft.Xna.Framework;
using EQuestLib;
using EQuestClient.Game;

namespace EQuestClient.Network
{
	public class EnterZonePacketHandler
	{
		private GameApp _game;
		
		public EnterZonePacketHandler(GameApp game)
		{
			_game = game;
		}
		
		public void HandleUpdateKnownItems(PacketIn packet)
		{
			int count = packet.ReadInt();

			for (int i = 0; i < count; i++)
			{
				var item = new Item(_game.GameScene);
				item.ID = packet.ReadInt();
				item.Name = packet.ReadString();
				item.Icon = packet.ReadInt();
				_game.GameScene.KnownItems.AddItem(item);
			}
		}

		public void HandleEnterZone(PacketIn packet)
		{
			string zoneName = packet.ReadString();
			string longName = packet.ReadString();
			int mobId = packet.ReadInt();
			Vector3 position = packet.ReadVector3();
			float heading = packet.ReadFloat();
			int inventoryItemsCount = packet.ReadInt();

			_game.GameScene.Player.Inventory.Clear();

			for (int i = 0; i < inventoryItemsCount; i++)
			{
				var slot = packet.ReadInt();
				var itemId = packet.ReadInt();
				_game.GameScene.Player.Inventory.Add(slot, itemId);
			}

			_game.EnterZone(zoneName, longName, mobId, position, heading);
		}
	}
}
