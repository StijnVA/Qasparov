using System;
using SDK = org.qasparov.qbis.SDK;

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
			var server = new SDK.QBisServer (
				             arguments.Host,
				             arguments.Port,
				             arguments.User,
				             arguments.Pass
			             );
			server.Connect ();
		
			Console.ReadLine ();
		}
	}
}
