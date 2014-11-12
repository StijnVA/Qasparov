using System;
using org.qasparov.qbis.server.qbus;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Threading;
using org.qasparov.qbis.server.host;
using System.Net;
using org.qasparov.qbis.server.logging;
using System.Collections.Concurrent;
using SDK = org.qasparov.qbis.SDK;

namespace org.qasparov.qbis.server.host
{
	public class QBisHost
	{
		QBusController controller;

		public Int32 Port { get; set; }
		public X509Certificate2 X509ServerCertificate { get; set; }
		private TcpListener listener;

		ConcurrentQueue<SDK.QBisMessage> messageQueue = new ConcurrentQueue<SDK.QBisMessage>();

		public QBisHost ()
		{
			//Default Value
			this.Port = 8844;

		}

		~QBisHost()
		{
			DisconnectFromQBus ();
		}

		public void StartListening(String pathToX509Certificate, String certificatePassword) {
			Logger.Log ("StartListening...");
			this.X509ServerCertificate = new X509Certificate2(pathToX509Certificate, certificatePassword);
			this.listener = new TcpListener (IPAddress.Any, this.Port);
			listener.Start ();
			ListenForIncommingConnectionOnNewThread ();
						
		}

		private void ListenForIncommingConnectionOnNewThread(){
			var listenThread = new Thread (() => ListenForIncommingConnection(this));
			listenThread.Start ();
		}
	


		private void ListenForIncommingConnection(QBisHost host){
			Logger.Log ("ListenForIncommingConnection...");
			var tcpClient = host.listener.AcceptTcpClient ();
			ListenForIncommingConnectionOnNewThread ();
			var qBisClient = new QBisClient (tcpClient, X509ServerCertificate);
			qBisClient.OnMessageReceived += (SDK.QBisMessage message) => {
				this.messageQueue.Enqueue(message);
			};
			qBisClient.StartProcessing ();
		}


		public void StartListening(String pathToX509Certificate, String certificatePassword  ,Int32 port) {
			this.Port = port;
			StartListening (pathToX509Certificate, certificatePassword);
		}

		public void ConnectToQBus(String host, String port, String user, String pass){
			controller = new QBusController (host, port, user, pass);
			controller.Connect ();
		}

		public void DisconnectFromQBus ()
		{
			if (this.controller != null) {
				this.controller.Disconnect ();
			}
		}
	}
}

