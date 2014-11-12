using System;
using System.Security.Cryptography.X509Certificates;
using org.qasparov.qbis.server.logging;
using Qbus.Communication;

namespace org.qasparov.qbis.server.qbus
{
	/// <summary>
	/// Represents a QBis Server. Contains Configuration Properties
	/// </summary>
	public class QBusController
	{
		public enum ControllerStates {
			CONNECTING,
			CONNECTED,
			DISCONNECTING,
			DISCONNECTED
		}

		private Qbus.Communication.Controller qBusController; 

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

		private ControllerStates _state;
		public ControllerStates State { get {
			return this._state;
			} 
			internal set {
				//TODO Trigger onStateChangedEvent
				this._state = value;
			} 
		}



		public QBusController (String Address, String Port, String UserName, String UserPassword)
		{
			this.Address = Address;
			this.Port = Int32.Parse(Port);
			this.UserName = UserName;
			this.UserPassword = UserPassword;
			this.State = ControllerStates.DISCONNECTED;
		}


		public void Connect(){
			if (this.State == ControllerStates.DISCONNECTED) {
				if (this.qBusController == null) {
					this.qBusController = new Qbus.Communication.Controller { 
			
						Address = this.Address,
						TcpPort = this.Port,
						Login = this.UserName,
						Password = this.UserPassword
					};
				}
				startConnectionToQBusController ();
			} else {
				throw new ApplicationException ("Not in the Disconnected state");
			}
		}

		private void startConnectionToQBusController(){
			this.State = QBusController.ControllerStates.CONNECTING;

			Qbus.Communication.ConnectionManager.Instance.ConnectionChanged += HandleConnectionChanged;
			Qbus.Communication.ConnectionManager.Instance.CommandReceived += HandleCommandReceived;
			Qbus.Communication.ConnectionManager.Instance.onError += HandleOnError;

			Qbus.Communication.ConnectionManager.Instance.Connect (this.qBusController);
		}

		void HandleOnError (object sender, Qbus.Communication.ConnectionManager.ErrorEventArgs args)
		{
			Logger.Log ("In HandleOnError!");
			//Logger.Log (sender, Logger.LogLevels.VERBOSE);
			//Logger.Log (args, Logger.LogLevels.VERBOSE);

		}

		void HandleConnectionChanged (Qbus.Communication.ControllerCommunication cc)
		{
			if (cc != null) {
				TcpCommunication tc = (TcpCommunication)cc;	
				Logger.Log ("Connection Changed:");
				Logger.Log ("   Connected      : " + cc.Controller.Connected);
				Logger.Log ("   Hostname       : " + cc.Controller.HostName);
				Logger.Log ("   TcpPort        : " + cc.Controller.TcpPort);
				Logger.Log ("   TcpComm Status : " + tc.Status);
			}
			
		}

		void HandleCommandReceived (object sender, Qbus.Communication.CommandEventArgs e)
		{
			Logger.Log ("Command Received: " );	

			Logger.WriteProperties (e.Command, "     ");
			
		}


		public void Disconnect(){
			this.State = QBusController.ControllerStates.DISCONNECTING;
			Qbus.Communication.ConnectionManager.Instance.Disconnect (this.qBusController);
			Qbus.Communication.ConnectionManager.Instance.DisconnectAll ();
		}
	}
}

