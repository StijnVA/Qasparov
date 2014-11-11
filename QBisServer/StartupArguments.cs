using System;

namespace org.qasparov.qbis.server
{
	public class StartupArguments
	{
		public String Host { get; internal set; }
		public String Port { get; internal set; }
		public String User { get; internal set;}
		public String Pass { get;internal set;}

		public static StartupArguments Parse(string[] args){
			var result = new StartupArguments ();
			for(var i = 0; i < args.Length; i++){
				var currentArg = args [i];
				switch (currentArg) {
				case "-host":
					if (i + 1 < args.Length) {
						result.Host = args [i + 1];
					}
					break;
				case "-port":
					if (i + 1 < args.Length) {
						result.Port = args [i + 1];
					}
					break;
				case "-user":
					if (i + 1 < args.Length) {
						result.User = args [i + 1];
					}
					break;
				case "-pass":
					if (i + 1 < args.Length) {
						result.Pass = args [i + 1];
					}
					break;
				default:
					break;
				}
			}
			return result;
		}

		private StartupArguments ()
		{
	
		}
	}
}

