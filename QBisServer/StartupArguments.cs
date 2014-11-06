using System;

namespace org.qasparov.qbis.server
{
	public class StartupArguments
	{
		public String Host { get; }
		public String Port { get; }

		public static StartupArguments Parse(string[] args){
			for(var i = 0; i < args.Length; i++){
				var currentArg = args [i];
				switch (currentArg) {
				case "-host":
					if(i+1 < args.Length){
						this.Host = args [i + 1];
					}
				case "-port":
					if(i+1 < args.Length){
						this.Port = args [i + 1];
					}
				default:
					break;
				}

			}
		}

		private StartupArguments ()
		{
	
		}
	}
}

