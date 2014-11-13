using System;

namespace org.qasparov.qbis.SDK
{
	/// <summary>
	/// QBisMessage is an message that will be send to the selected QBisClients.
	/// </summary>
	public class QBisMessage
	{
		//TODO: Think about it
		enum SecurityLevel {
			PUBLIC,
			INFORMATIVE,
			CRITICAL
		}

		//TODO Think about it
		enum TypeOfMessage {
			TEMPERATURE,
			OTHER
		}

		public String Desciption { get; set; }


		public QBisMessage ()
		{

		}
	}
}

