using System;
using System.Configuration;

namespace org.qasparov.qbis.SDK
{
	/// <summary>
	/// Used by the SDK components to read the application config file.
	/// It can be set by providing a configuration with your application (example: myApp.exe.config)
	/// but the values can also be provided from code
	/// </summary>
	public class ApplicationConfiguration
	{
		private static ApplicationConfiguration instance;
		private static Object thisLock = new Object();

		public static ApplicationConfiguration Instance { get {
				lock (thisLock) {
					if(instance==null){
						instance = new ApplicationConfiguration ();
					}
					return instance;
				}
			} }

		public String x509CertificatePath { get; set; }
		public String qBisHostName { get; set; }
		public String qBisPort { get; set; }

		protected ApplicationConfiguration ()
		{
			//Default Values
			this.qBisHostName = "localhost";
			this.qBisPort = "8844";

			foreach (var propertInfo in this.GetType().GetProperties()) {
				if (propertInfo.CanWrite) {
					var value = ConfigurationManager.AppSettings [propertInfo.Name];
					if (value != null) {
						propertInfo.SetValue (this, value, null);
					}
				}
			}
		}
	}
}

