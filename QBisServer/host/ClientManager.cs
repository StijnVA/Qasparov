using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SDK = org.qasparov.qbis.SDK;

namespace org.qasparov.qbis.server.host
{
	public class ClientManager
	{
		List<QBisClient> clients = new List<QBisClient>();

		public ClientManager ()
		{
		}

		public void Register (QBisClient qBisClient)
		{
			this.clients.Add (qBisClient);
		}

		public void Unregister (QBisClient qBisClient)
		{
			this.clients.Remove (qBisClient);
		}

		public void Publish (org.qasparov.qbis.SDK.QBisMessage message)
		{
			//TODO: Evaluete if the client is allowed to receive the message 
			clients.ForEach((client) => {
				if(client!=null){
					client.ProcessMessage(message);
				}
			});
		}

		public void CloseAll(){
			clients.ForEach ((client) => {
				client.Stop();
			});
		}
	}
}

