using System;
using org.qasparov.qbis.server.qbus;
using org.qasparov.qbis.server.host;


namespace org.qasparov.qbis.server
{
	/// <summary>
	/// Servise host is the application to hast the services of the QBis System. This application is intended to run as a deamon
	/// </summary>
	class ServiseHost
	{
		public static void Main (string[] args)
		{
			var arguments = StartupArguments.Parse (args);

			//Console.Write("Certificate Password:");
			//var pass = Console.ReadLine ();

			var host = new QBisHost ();
			host.ConnectToQBus (
				arguments.Host,
				arguments.Port,
				arguments.User,
				arguments.Pass);

			//TO Create a self signed test certificat:
			//openssl req -x509 -newkey rsa:2048 -keyout key.pem -out cert.pem -days 365
			//openssl pkcs12 -export -out selfSigned.pfx - inkey key.pem -in cert.pem


			host.StartListening ("selfSigned.pfx", "mypass");

			Console.ReadLine ();

			host.DisconnectFromQBus ();


		}
	}
}
