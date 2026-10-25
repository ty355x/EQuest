using System;
using System.Collections.Generic;
using EQuestLib;

namespace EQuestClient.Network
{
	public abstract class PacketHandlerBase
	{
		protected Server _server;

		public PacketHandlerBase(Server server)
		{
			_server = server;
		}

		public void ReadPackets()
		{
			lock (_server.Lock)
			{
				while (_server.HasPackets)
				{
					List<PacketIn> packets = _server.GetPackets();

					foreach (PacketIn packet in packets)
						HandleCommand((ServerCommand)packet.ReadCommand(), packet);

					packets.Clear();
				}
			}
		}

		protected abstract void HandleCommand(ServerCommand command, PacketIn packet);
	}
}
