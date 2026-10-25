using System;
using EQuestLib;

namespace EQuestServer
{
	public class SocketHandler : ISocketHandler
	{
		private ServerThread _serverThread;
		
		public SocketHandler(ServerThread serverThread)
		{
			_serverThread = serverThread;
		}

		public void SocketDataReceived(object sender, PacketIn packet)
		{
			_serverThread.QueuePacket(packet);
		}

		public void SocketDisconnected(object sender)
		{
			Client client = (Client)sender;
			_serverThread.HandleClientDisconnect(client);
		}
	}
}
