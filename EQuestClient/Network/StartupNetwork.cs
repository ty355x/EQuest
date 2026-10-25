using System;
using EQuestLib;

namespace EQuestClient.Network
{
	public enum StartupNetworkState { Connecting, EnteringZone }

	public class StartupNetwork : PacketHandlerBase
	{
		public StartupNetworkState State { get; private set; }
		private EnterZonePacketHandler _enterZoneHandler;
		private GameApp _game;
				
		public StartupNetwork(GameApp app, Server server) : base(server)
		{
			_game = app;
			_enterZoneHandler = new EnterZonePacketHandler(_game);
			State = StartupNetworkState.Connecting;
		}

		public void Update()
		{
			base.ReadPackets();
		}

		protected override void HandleCommand(ServerCommand command, PacketIn packet)
		{
			if (command == ServerCommand.RequestLogin)
				HandleRequestLogin(packet);
			else if (command == ServerCommand.EnterZone)
				HandleEnterZone(packet);
			else if (command == ServerCommand.UpdateKnownItems)
				HandleUpdateKnownItems(packet);
			else
				Log.Debug("Got unrecognized server command: " + command);
		}

		public void HandleRequestLogin(PacketIn packet)
		{
			var response = PacketMaker.MakeLogin(_game.Login.Email, _game.Login.Password);
			_server.Send(response);
		}

		private void HandleEnterZone(PacketIn packet)
		{
			State = StartupNetworkState.EnteringZone;
			_enterZoneHandler.HandleEnterZone(packet);
		}

		private void HandleUpdateKnownItems(PacketIn packet)
		{
			_enterZoneHandler.HandleUpdateKnownItems(packet);
		}
	}
}
