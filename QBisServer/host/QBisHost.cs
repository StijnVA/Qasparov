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

namespace org.qasparov.qbis.server.host
{

	/// <summary>
	/// The QBisHost is responsible for hosting the services.
	/// Those are: A (one at this time) QBusController and QBisClients (one for each Client connected).   
	/// </summary>
	public class QBisHost
	{
		QBusController controller;

		public Int32 Port { get; set; }
		public X509Certificate2 X509ServerCertificate { get; set; }
		private TcpListener listener;

		private MessageDispatcher messageDispatcher = new MessageDispatcher();

		public delegate void NewClientConnectedEvent (QBisHost sender, QBisClient client);
		public NewClientConnectedEvent OnNewClientConnected;

		public delegate void ClientDisconnectedEvent (QBisHost sender, QBisClient client);
		public ClientDisconnectedEvent OnClientDisconnected;

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
			qBisClient.OnInstructionReceived += (QBisClient sender, SDK.QBisInstruction instruction) => {
				Logger.Log(String.Format("Instruction from {0} received: {1}", sender.ToString(), instruction.Desciption));

				//Only intended for test purposel
				//TODO: Remove
				if(instruction.Desciption == SDK.QBisProtocolMessages.BROADCAST_TEST){
					messageDispatcher.Publish(new SDK.QBisMessage {
						Desciption = "Broadcasting the broadcast request"
					});
				}  
			};
			if (OnNewClientConnected != null) {
				OnNewClientConnected (this, qBisClient);
			}
			messageDispatcher.Register (qBisClient);
			qBisClient.StartProcessing ();
			messageDispatcher.Unregister (qBisClient);
			if (OnClientDisconnected != null) {
				OnClientDisconnected (this, qBisClient);
			}
		}

		public void StartListening(String pathToX509Certificate, String certificatePassword  ,Int32 port) {
			this.Port = port;
			StartListening (pathToX509Certificate, certificatePassword);
		}

		public void ConnectToQBus(String host, String port, String user, String pass){
			controller = new QBusController (host, port, user, pass);
			controller.OnStateChange += ((sender, from, to) => {
				Logger.Log(String.Format("{0} is changing state from {1} to {2}",sender.ToString(), from.ToString(), to.ToString()));
			});
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

