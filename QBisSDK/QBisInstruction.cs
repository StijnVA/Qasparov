using System;

namespace org.qasparov.qbis.SDK
{
	/// <summary>
	/// A QBisInstruction is an instruction received from a client application.
	/// </summary>
	public class QBisInstruction
	{
		/// <summary>
		/// An optional Description that explains the meaning of the instruction in a human readable form. Can be used for display 
		/// or loggin purpose.
		/// </summary>
		/// <value>The desciption.</value>
		public String Desciption { get; set; }

		/// <summary>
		/// A protocol message is an instruction that will be inter
		/// </summary>
		/// <value>The protocol messages.</value>
		public QBisProtocolMessages ProtocolMessages { get; set; }

		public QBisInstruction ()
		{

		}
	}
}

