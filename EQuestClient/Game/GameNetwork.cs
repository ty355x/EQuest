using System;
using Microsoft.Xna.Framework;

using EQuestLib;
using EQuestClient.Network;

namespace EQuestClient.Game
{
	public class GameNetwork
	{
		private const double ClientUpdateIntervalMs = 200;

		private GameApp _game;
		private GameScene _scene;
		private Server _server;
		private GamePacketHandler _packetHandler;
		private double _lastClientStateUpdateMs = -1;
		
		public GameNetwork(GameApp game, GameScene scene, Server server)
		{
			_game = game;
			_scene = scene;
			_server = server;
			_packetHandler = new GamePacketHandler(game, _scene, _server);
		}

		public void Update(GameTime time)
		{
			if (!_server.IsConnected)
			{
				_scene.ConnectionLost();
				return;
			}

			_packetHandler.ReadPackets();
			CheckUpdateClientState(time);
		}

		public void SendMessage(string message)
		{
			PacketOut packet = PacketMaker.MakeMessage(message);
			_server.Send(packet);
		}

		public void SendPacket(PacketOut packet)
		{
			_server.Send(packet);
		}

		private void CheckUpdateClientState(GameTime time)
		{
			if (Util.HasTimeElapsed(time, _lastClientStateUpdateMs, ClientUpdateIntervalMs))
			{
				_lastClientStateUpdateMs = time.TotalGameTime.TotalMilliseconds;

				PacketOut packet = PacketMaker.MakeMovementUpdate(_scene.Player);
				_server.Send(packet);
			}
		}
	}
}
