using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SDK = org.qasparov.qbis.SDK;

namespace org.qasparov.qbis.server.host
{
	public class MessageDispatcher
	{
		List<QBisClient> clients = new List<QBisClient>();

		public MessageDispatcher ()
		{
		}


		public void Publish (org.qasparov.qbis.SDK.QBisMessage message)
		{
			clients.ForEach((client) => {
				if(client!=null){
					client.ProcessMessage(message);
				}
			});
		}
	}
}

