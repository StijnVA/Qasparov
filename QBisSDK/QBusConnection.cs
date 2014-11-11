using System;
using QBusSDK = Qbus.Communication.SDK;
using System.Threading;

namespace org.qasparov.qbis.SDK
{
	class QBusConnection
	{
		private Qbus.Communication.Controller qBusControler; 
		private QBisServer qBisServer;

		public QBusConnection(QBisServer server){
			this.qBisServer = server;
			this.qBusControler = new Qbus.Communication.Controller ();
			this.qBusControler.Address = server.Address;
			this.qBusControler.TcpPort = server.Port;
			this.qBusControler.Login = server.UserName;
			this.qBusControler.Password = server.UserPassword;
		}

		public void Connect(){
			this.qBisServer.State = QBisServer.ServerStates.CONNECTING;
			Qbus.Communication.ConnectionManager.Instance.Connect (this.qBusControler);
		}

		public void Disconnect(){
			this.qBisServer.State = QBisServer.ServerStates.DISCONNECTING;
			Qbus.Communication.ConnectionManager.Instance.Disconnect (this.qBusControler);
		}
	}
}
