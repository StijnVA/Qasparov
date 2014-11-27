using System;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Text;
using System.Security.Authentication;

namespace org.qasparov.qbis.server
{
	/// <summary>
	/// Command line application to communicate with the QBisServer
	/// </summary>
	class QBis
	{
		public static void Main (string[] args)
		{
			try {


				//We don't store te password in the config file, so we need to set it in code.
				Console.WriteLine ("Password to unlock the key:");			
				var pass = ReadPassword ();
				SDK.QBisClientApplicationConfiguration.Instance.x509CertificatePass = pass;

				var qBisClient = new SDK.QBisClient ();

				qBisClient.Connect ();
				qBisClient.OnMessageReceived += (sender, message) => {
					Console.WriteLine (String.Format ("Message received from {0}: {1}", sender.Host, message.Desciption));
				};
			
				qBisClient.StartReceiving ();

				var line = Console.ReadLine ();
				while (line != "exit") {
					qBisClient.SendInstruction (new SDK.QBisInstruction { Desciption = line });
					line = Console.ReadLine ();
				}

			} catch (Exception ex) {
				Console.WriteLine ("Error: " + ex.Message);
			}


		}

		/// <summary>
		/// Reads the password.
		/// </summary>
		/// <returns>The password.</returns>
		/// (c) by Parth Shah
		/// http://www.c-sharpcorner.com/forums/thread/32102/password-in-C-Sharp-console-application.aspx
		public static string ReadPassword ()
		{
			string password = "";
			ConsoleKeyInfo info = Console.ReadKey (true);
			while (info.Key != ConsoleKey.Enter) {
				if (info.Key != ConsoleKey.Backspace) {
					Console.Write ("*");
					password += info.KeyChar;
				} else if (info.Key == ConsoleKey.Backspace) {
					if (!string.IsNullOrEmpty (password)) {
						// remove one character from the list of password characters
						password = password.Substring (0, password.Length - 1);
						// get the location of the cursor
						int pos = Console.CursorLeft;
						// move the cursor to the left by one character
						Console.SetCursorPosition (pos - 1, Console.CursorTop);
						// replace it with space
						Console.Write (" ");
						// move the cursor to the left by one character again
						Console.SetCursorPosition (pos - 1, Console.CursorTop);
					}
				}
				info = Console.ReadKey (true);
			}
			// add a new line because user pressed enter at the end of their password
			Console.WriteLine ();
			return password;
		}
	}
}
