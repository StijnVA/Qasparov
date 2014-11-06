using System;

namespace org.qasparov.qbis.SDK
{
	/// <summary>
	/// Represents a QBis Server. Contains Configuration Properties
	/// </summary>
	public class Server
	{
		enum ServerStates {
			CONNECTED,
			DISCONNECTED
		}

		//TODO 
		public String Port { get; set; }
		public String Address { get; set; }
		public X509Certificate2 X509Certificate { get; set; }
		public ServerStates State { get; set;}

		public Server (String Address, String Port)
		{
			this.Address = Address;
			this.Port = Port;
		}
	}
}

