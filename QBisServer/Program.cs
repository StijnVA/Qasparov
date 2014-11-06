using System;
using SDK = org.qasparov.qbis.SDK;

namespace org.qasparov.qbis.server
{
	/// <summary>
	/// Servise host is the application to hast the services of the QBis System. This application is intended to 
	/// </summary>
	class ServiseHost
	{
		public static void Main (string[] args)
		{
			var arguments = StartupArguments.Parse (args);
			var server = new SDK.Server (
				arguments.Host,
				arguments.Port
			);


	}
}
