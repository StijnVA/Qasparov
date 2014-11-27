using System;
using System.Configuration;

namespace org.qasparov.qbis.SDK
{
	/// <summary>
	/// Used by the SDK components to read the application config file.
	/// It can be set by providing a configuration with your application (example: myApp.exe.config)
	/// but the values can also be provided from code
	/// </summary>
	public abstract class ApplicationConfiguration<T> where T: ApplicationConfiguration<T> , new()
	{

		private static T instance;
		private static Object thisLock = new Object();

		public static T Instance { get {
				lock (thisLock) {
					if(instance==null){
						instance = new T();
					}
					return instance;
				}
			} 
		}
			
		protected abstract void SetDefaultValues();

		protected ApplicationConfiguration ()
		{
			SetDefaultValues ();

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

