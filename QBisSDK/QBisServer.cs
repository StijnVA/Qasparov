using System;
using System.Security.Cryptography.X509Certificates;

namespace org.qasparov.qbis.SDK
{
	/// <summary>
	/// Represents a QBis Server. Contains Configuration Properties
	/// </summary>
	public class QBisServer
	{
		public enum ServerStates {
			CONNECTING,
			CONNECTED,
			DISCONNECTING,
			DISCONNECTED
		}

		//TODO 
		public Int32 Port { get; set; }
		public String Address { get; set; }

		public string UserName {
			get;
			set;
		}

		public string UserPassword {
			get;
			set;
		}

		public X509Certificate2 X509Certificate { get; set; }
		public ServerStates State { get;  internal set; }

		private QBusConnection qBisConnection;

		public QBisServer (String Address, String Port, String UserName, String UserPassword)
		{
			this.Address = Address;
			this.Port = Int32.Parse(Port);
			this.UserName = UserName;
			this.UserPassword = UserPassword;
			this.State = ServerStates.DISCONNECTED;
		}


		public void Connect(){
			if (this.State == ServerStates.DISCONNECTED) {
				if (this.qBisConnection == null) {
					this.qBisConnection = new QBusConnection (this);
				}
				this.qBisConnection.Connect ();
			} else {
				throw new ApplicationException ("Not in the Disconnected state");
			}
		}
	}
}

