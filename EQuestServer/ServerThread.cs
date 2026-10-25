using System;
using System.Collections.Generic;
using System.Threading;

using EQuestLib;

namespace EQuestServer
{
	/// <summary>
	/// Handles the packet queue for a server and manages the thread to call the server update method.
	/// </summary>
	public class ServerThread
	{
		public object PacketQueueLock = new object();
		public int PacketCount { get { return _packets.Count; } }
		
		private IServer _server;
		private int _updateIntervalMS;
		private string _serverName;
		
		private Thread _thread;
		private bool _isRunning = false;
		private List<PacketIn> _packets = new List<PacketIn>();
		private Timer _timer = new Timer();
		

		public ServerThread(IServer server, int updateIntervalMS, string serverName)
		{
			_server = server;
			_serverName = serverName;
			_updateIntervalMS = updateIntervalMS;
			_thread = new Thread(Run);
		}

		public void Start()
		{
			if (!_isRunning)
			{
				_isRunning = true;
				_thread.Start();
			}
		}

		public void Stop(bool blockUntilTerminated)
		{
			_isRunning = false;

			if (blockUntilTerminated && _thread.IsAlive)
				_thread.Join();
		}

		public void QueuePacket(PacketIn packet)
		{
			lock (PacketQueueLock)
				_packets.Add(packet);
		}

		public void HandleClientDisconnect(Client client)
		{
			_server.HandleClientDisconnect(client);
		}
		
		public PacketIn GetPacket(int index)
		{
			return _packets[index];
		}

		public void ClearPackets()
		{
			_packets.Clear();
		}
		
		private void Run()
		{
			Log.Info(_serverName + " - Starting server thread");
			_timer.Start();

			while (_isRunning)
			{
				long time = _timer.CurrentTime;
				_server.Update(time);
				Thread.Sleep(_updateIntervalMS);
			}

			_timer.Stop();
			Log.Info(_serverName + " - Server thread terminated");
		}
	}
}
