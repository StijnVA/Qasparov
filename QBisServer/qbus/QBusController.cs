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
			CHANGING_STATE,
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
			

		public delegate void StateChangedEvent(QBusController sender, ControllerStates from, ControllerStates to);
		public delegate void CommandReceivedEvent(QBusController sender, SDK.QBisMessage qBisMessage);

		public StateChangedEvent OnStateChange;
		public CommandReceivedEvent OnCommandReceived;



		private ControllerStates _state;
		public ControllerStates State { get {
				if (this._state == ControllerStates.CHANGING_STATE) {
					throw new ApplicationException ("You should not care about the state when changing state.");
				}
				return this._state;
			} 
			internal set {
				if (this._state == ControllerStates.CHANGING_STATE) {
					throw new ApplicationException ("You should not care about the state when changing state.");
				}
				var oldstate = this._state;
				this._state = ControllerStates.CHANGING_STATE;
				if (OnStateChange != null && oldstate != value) {
					OnStateChange.Invoke (this, oldstate, value);
				}
				this._state = value;
			} 
		}


		public QBusController (String Address, String Port, String UserName, String UserPassword)
		{
			this.Address = Address;
			this.Port = Int32.Parse(Port);
			this.UserName = UserName;
			this.UserPassword = UserPassword;
			this._state = ControllerStates.DISCONNECTED; //When using the setter, this would cause an exception bacause of the default value. 
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

		public void ProcessAction (org.qasparov.qbis.SDK.QBisAction action)
		{
			lock (this) {
				//TODO: Process the action.
			}
		}

		void HandleOnError (object sender, Qbus.Communication.ConnectionManager.ErrorEventArgs args)
		{
			Logger.Log (String.Format("{0}({1})", args.Exception.Message, args.Exception.Source), Logger.LogLevels.WARNING); 
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
			
				switch (tc.Status) {
				case TCP_STATUS.CONNECTED:
					this.State = ControllerStates.CONNECTED;
					break;
				case TCP_STATUS.DISCONNECTED:
					this.State = ControllerStates.DISCONNECTED;
					break;
				default:
					break;
				}

			}
		}

		void HandleCommandReceived (object sender, Qbus.Communication.CommandEventArgs e)
		{
			Logger.Log ("Command Received: " );	
			Logger.WriteProperties (e.Command, "     ");

			if (OnCommandReceived != null) {
				this.OnCommandReceived(this, new SDK.QBisMessage{
					//TODO: construct a meaninfull Message
					Desciption = e.Command.ToString()
				});
			}

		}


		public void Disconnect(){
			this.State = QBusController.ControllerStates.DISCONNECTING;
			Qbus.Communication.ConnectionManager.Instance.Disconnect (this.qBusController);
			Qbus.Communication.ConnectionManager.Instance.DisconnectAll ();
		}
	}
}

