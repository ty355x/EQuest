using System;

namespace EQuestLib
{
	public interface ISocketHandler
	{
		void SocketDataReceived(object sender, PacketIn packet);
		void SocketDisconnected(object sender);
	}
}
