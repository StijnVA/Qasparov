using System;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using org.qasparov.qbis.server.logging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace org.qasparov.qbis.server.host
{
	public class QBisClient
	{
		private TcpClient tcpclient;
		private X509Certificate2 x509certificate;

		public QBisClient (TcpClient tcpclient, X509Certificate2 x509certificate)
		{
			this.tcpclient = tcpclient;
			this.x509certificate = x509certificate;
		}

		public void StartProcessing(){
			SslStream sslStream = new SslStream (tcpclient.GetStream (), false);
			try 
			{			
				sslStream.AuthenticateAsServer(x509certificate, false, SslProtocols.Tls, true);
				var reader = new StreamReader(sslStream);
				Logger.Log("Ready to server!");
				var line = reader.ReadLine();
				while(line!= null && !line.Equals(SDK.QBisProtocolMessages.TERMINATE_SESSION)){
					ProcessLine(line);
					line = reader.ReadLine();
				}
			}
			finally
			{
				sslStream.Close();
				tcpclient.Close();
			}
		}

		protected void ProcessLine(String line){
			Logger.Log(String.Format("Receive from client: {0}", line));
			var msg = new SDK.QBisMessage { Desciption = line };
			OnMessageReceived (msg);
		}

		public delegate void OnMessageReceivedDelegate(SDK.QBisMessage message);
		public OnMessageReceivedDelegate OnMessageReceived;

	}
}

