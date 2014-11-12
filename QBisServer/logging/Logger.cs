using System;
using System.Text;
using System.Collections;
using System.Threading;

namespace org.qasparov.qbis.server.logging
{
	public class Logger
	{
		#region Singletone
		private static Logger instance = new Logger();

		private Logger ()
		{
		}

		public static Logger get() {
				return instance;

		}

		#endregion

		public enum LogLevels {
			VERBOSE,
			DEBUG,
			WARNING,
			ERROR,
			CRITICAL
		}
	
		static public Logger Log(String message, LogLevels level = LogLevels.VERBOSE){
			return instance.Write (level, message);	
		}

		static public void WriteProperties(Object obj, String prefix = ""){
			foreach (var prop in obj.GetType().GetProperties()) {
				var val = prop.GetValue (obj, null);
				String text;
				if (val == null) {
					text = "<NULL>";
				} else if (val is IEnumerable) {
					StringBuilder bldr = new StringBuilder ();
					bldr.Append ("{"); 
					foreach (var item in (IEnumerable)val) {
						bldr.Append (item).Append (",");
					}
					if (bldr.Length > 2) {
						//Remove last comma
						bldr.Remove (bldr.Length - 1, 1);					
					} 
					bldr.Append ("}");
					text = bldr.ToString ();
				} else {
					text = val.ToString ();
				}
				Log(String.Format("{2}{0}={1}", prop.Name, text, prefix));
			}
		}

		static public Logger Log(Object obj, LogLevels level = LogLevels.VERBOSE){
			StringBuilder bldr = new StringBuilder ();
			if (obj == null) {			
				bldr.AppendLine ("Object is NULL");
			} else {
				bldr.AppendLine (String.Format ("Object is of type {0}", obj.GetType ().Name));
			}
			return instance.Write (level, bldr.ToString());	
		}

		public Logger Write(LogLevels level = LogLevels.VERBOSE, String message = ""){
			Console.WriteLine (String.Format ("({2,3}){0,10}:{1}",
			                                  level.ToString (),
			                                  message,
			                                  Thread.CurrentThread.ManagedThreadId));	
			return this;
		}
	}
}

