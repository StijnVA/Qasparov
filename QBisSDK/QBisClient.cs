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
		public String HostName { get; private set; }

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
			Host = QBisClientApplicationConfiguration.Instance.qBisHostAddress;
			Port = Int32.Parse(QBisClientApplicationConfiguration.Instance.qBisPort);
			x509Certificate = new X509Certificate2 (
				QBisClientApplicationConfiguration.Instance.x509CertificatePath, 
				QBisClientApplicationConfiguration.Instance.x509CertificatePass);
			HostName = QBisClientApplicationConfiguration.Instance.qBisHostName;
		}

		public void Connect ()
		{
			TcpClient tcpClient = new TcpClient (this.Host, this.Port);
			SslStream sslStream = new SslStream (tcpClient.GetStream (), false, (sender, certificate, chain, sslPolicyErrors) => {

				if (sslPolicyErrors == SslPolicyErrors.None){
					Console.WriteLine("Certificate OK");
					return true;
				}

				Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

				// Do not allow this client to communicate with unauthenticated servers. 
				return false;

			},	
			(sender, targetHost, localCertificates, remoteCertificate, acceptableIssuers) =>{
				Console.WriteLine("Client is selecting a local certificate.");
				Console.WriteLine("   for target host: " + targetHost);
				Console.WriteLine(localCertificates[0].Subject);
				return localCertificates[0];
			});
			sslStream.AuthenticateAsClient (this.HostName,
				new X509Certificate2Collection(this.x509Certificate), 
				System.Security.Authentication.SslProtocols.Tls,
				false);
			this.writer = new StreamWriter (sslStream);
			this.reader = new StreamReader (sslStream);
		}

		public void SendInstruction(QBisInstruction instruction){
			if (this.writer != null) {
				this.writer.WriteLine (instruction.Desciption);
				this.writer.Flush ();
			} else {
				throw new ApplicationException ("No stream to write to");
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

