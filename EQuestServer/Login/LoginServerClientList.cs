using System;
using System.Collections.Generic;

namespace EQuestServer.Login
{
	class LoginServerClientList
	{
		private List<Client> _clients = new List<Client>();
		private List<Client> _clientAddQueue = new List<Client>();
		private List<Client> _clientRemoveQueue = new List<Client>();

		public void AddClient(Client client)
		{
			lock (_clientAddQueue)
				_clientAddQueue.Add(client);
		}

		public void RemoveClient(Client client)
		{
			lock (_clientRemoveQueue)
				_clientRemoveQueue.Add(client);
		}

		public void ProcessClientQueue(ServerThread serverThread)
		{
			ProcessClientRemoveQueue();
			ProcessClientAddQueue(serverThread);
		}

		private void ProcessClientAddQueue(ServerThread serverThread)
		{
			lock (_clientAddQueue)
			{
				foreach (var client in _clientAddQueue)
				{
					client.SocketHandler = new SocketHandler(serverThread);
					client.State = ClientState.PendingLogin;
					_clients.Add(client);
					client.Send(PacketMaker.MakeRequestLogin());
				}

				_clientAddQueue.Clear();
			}
		}

		private void ProcessClientRemoveQueue()
		{
			lock (_clientRemoveQueue)
			{
				foreach (var client in _clientRemoveQueue)
				{
					client.State = ClientState.Disconnected;
					client.Dispose();
					_clients.Remove(client);
				}

				_clientRemoveQueue.Clear();
			}
		}
	}
}
