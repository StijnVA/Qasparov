using System;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.ComponentModel;

namespace org.qasparov.qbis.SDK
{
	public class QBisClient
	{
		public X509Certificate2 x509Certificate { get; private set; }
		public String Host { get; private set;}
		public Int32 Port { get; private set; }

		public delegate void MessageReceivedEvent (QBisClient sender, QBisMessage QBisMessage);
		public MessageReceivedEvent OnMessageReceived;

		private StreamWriter writer;
		private StreamReader reader;

		public QBisClient (String host, Int32 port, X509Certificate2 x509Certificate)
		{
			this.Host = host;
			this.Port = port;
			this.x509Certificate = x509Certificate;
		}

		/// <summary>
		/// Construct a QBisClient based on the ApplicationConfiguration values.
		/// </summary>
		public QBisClient ()
		{
			Host = ApplicationConfiguration.Instance.qBisHostName;
			Port = Int32.Parse(ApplicationConfiguration.Instance.qBisPort);
			x509Certificate = new X509Certificate2 (ApplicationConfiguration.Instance.x509CertificatePath, "mypass");
		}

		public void Connect ()
		{
			TcpClient tcpClient = new TcpClient (this.Host, this.Port);
			SslStream sslStream = new SslStream (tcpClient.GetStream ());
			sslStream.AuthenticateAsClient (this.Host,
				new X509Certificate2Collection(this.x509Certificate), 
				System.Security.Authentication.SslProtocols.Tls,
				false);
			this.writer = new StreamWriter (sslStream);
			this.reader = new StreamReader (sslStream);
		}

		public void SendInstruction(QBisInstruction instruction){
			if (this.writer != null) {
				this.writer.WriteLine (instruction.Desciption);
			}
		}

		public void StartReceiving(){
			BackgroundWorker bw = new BackgroundWorker ();
			bw.DoWork += (sender, e) => {
				while(!reader.EndOfStream){
					var line = reader.ReadLine();
					var message = new QBisMessage {
						Desciption = line
					};
					if(OnMessageReceived!=null){
						OnMessageReceived(this,message);
					}
				}	
				Console.WriteLine("-- END OF STREAM --");
			};
			bw.RunWorkerAsync ();
		}
	}
}

