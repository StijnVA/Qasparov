using System;

namespace org.qasparov.qbis.SDK
{
	public class QBisClientApplicationConfiguration : ApplicationConfiguration<QBisClientApplicationConfiguration>
	{

		public String x509CertificatePath { get; set; }
		public String x509CertificatePass { get; set; }
		public String qBisHostAddress{ get; set; }
		public String qBisPort { get; set; }
		public String qBisHostName { get; set;}

		protected override void SetDefaultValues ()
		{
			this.qBisHostName = "localhost";
			this.qBisPort = "8844";
		}

		/// <summary>
		/// DO NOT USE THIS CONSTRUCTOR
		/// It need to be public due to the construction of it in his super class.
		/// Altougt this class is intended as a singleton, use QBisClientApplicationConfiguration.Instance instead
		/// </summary>
		public QBisClientApplicationConfiguration () : base()
		{

		}
	}
}

