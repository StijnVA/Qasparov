using System;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Text;
using System.Security.Authentication;

namespace org.qasparov.qbis.server
{
	/// <summary>
	/// Command line application to communicate with the QBisServer
	/// </summary>
	class QBis
	{
		public static void Main (string[] args)
		{
			try{
			SDK.ApplicationConfiguration.Instance.x509CertificatePath = "QBisClient.pfx";

			//The follow code need to be in SDK.QBisClient
			//Currently developed in an other branch

			Console.ReadLine ();

			var tcpClient = new TcpClient (
				SDK.ApplicationConfiguration.Instance.qBisHostName,
				//SDK.ApplicationConfiguration.Instance.qBisPort
				8844) ;
			var sslStream = new SslStream (tcpClient.GetStream (),
				                               false, 
				                               (sender, certificate, chain, sslPolicyErrors) => {
	
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

				Console.WriteLine("Before Authenticating");
				try{
					sslStream.AuthenticateAsClient (
						"QBis Server",
						new X509CertificateCollection (new []  { 
							new X509Certificate2 (
								SDK.ApplicationConfiguration.Instance.x509CertificatePath,
								"mypass") } ),
						System.Security.Authentication.SslProtocols.Tls,
						true);
				}catch (Exception e)
				{
					Console.WriteLine("Exception: {0}", e.Message);
					if (e.InnerException != null)
					{
						Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
					}
					Console.WriteLine ("Authentication failed - closing the connection.");
					tcpClient.Close();
					return;
				}

				Console.WriteLine("Authenticated");

				Console.WriteLine(sslStream.RemoteCertificate.Subject);


			//var writer = new StreamWriter (sslStream);		
			byte[] messsage = Encoding.UTF8.GetBytes("Hello world");
			sslStream.Write(messsage);
			sslStream.Flush();

			}catch(Exception ex){
				Console.WriteLine (ex.Message);
			}


		}
	}
}
