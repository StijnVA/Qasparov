using System;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using org.qasparov.qbis.server.logging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Text;

namespace org.qasparov.qbis.server.host
{
	public class QBisClient
	{
		public const String ANONYMOUS = "ANONYMOUS"; 

		private TcpClient tcpclient;
		private X509Certificate2 x509certificate;
		private StreamWriter writer;
		private X509Certificate2 x509clientCertificate;

		public delegate void ClientConnectedEvent (QBisClient sender);
		public ClientConnectedEvent OnClientConnected;

		public delegate void InstructionReceivedDelegate(QBisClient sender, SDK.QBisInstruction instruction);
		public InstructionReceivedDelegate OnInstructionReceived;

		public QBisClient (TcpClient tcpclient, X509Certificate2 x509certificate)
		{
			this.tcpclient = tcpclient;
			this.x509certificate = x509certificate;
		}
			
		public X509Certificate2 ClientCertificate{
			get {
				return this.x509clientCertificate;
			}
		}

		public String FriendlyName {
			get { 
				if (this.x509clientCertificate == null) {
					return ANONYMOUS;
				} else {
					if (!String.IsNullOrWhiteSpace (this.x509clientCertificate.FriendlyName)) {
						return this.x509clientCertificate.FriendlyName;
					}else{
						return this.x509clientCertificate.Subject;
					}
				}
			}
		}

		public void StartProcessing(){
			SslStream sslStream = new SslStream (tcpclient.GetStream (), false);
			try 
			{			
				sslStream.AuthenticateAsServer(x509certificate, true, SslProtocols.Tls, true);
				if(sslStream.RemoteCertificate!=null){
					Logger.Log("A certificate was provided by the client.");
					this.x509clientCertificate = new X509Certificate2(sslStream.RemoteCertificate);
				}else{
					Logger.Log("There was NO certificate provided by the client.");
				}
				if(this.OnClientConnected!=null){
					this.OnClientConnected(this);
				}
				var reader = new StreamReader(sslStream);
				this.writer = new StreamWriter(sslStream);
				Logger.Log("Ready to server!");
				var line = reader.ReadLine();
				while(line!= null && !line.Equals(SDK.QBisProtocolMessages.TERMINATE_SESSION)){
					ProcessLine(line);
					line = reader.ReadLine();
				}
			}catch(Exception ex){
				Logger.Log (ex.Message, Logger.LogLevels.ERROR);
			}
			finally
			{
				sslStream.Close();
				tcpclient.Close();
			}
		}

		protected void ProcessLine(String line){
			Logger.Log(String.Format("Receive from client: {0}", line));
			var msg = new SDK.QBisInstruction { Desciption = line };
			if (OnInstructionReceived != null) {
				OnInstructionReceived (this, msg);
			}
		}

		public void ProcessMessage (SDK.QBisMessage message)
		{
			if (writer != null) {
				byte[] bytes = Encoding.UTF8.GetBytes(message.Desciption);
				writer.WriteLine (bytes);
				writer.Flush ();
			}
		}

		public override string ToString ()
		{
			return string.Format ("[QBisClient ({0})]", FriendlyName);
		}
	

	}
}

