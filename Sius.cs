/*
 * Sius
 * TCP User and Chat Database
 * Zachary Read © 2009. All Rights Reserved.
 */

using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Timers;

namespace Sius
{
	class Sius
	{
		/// <summary>
		/// The status of the program. It is used namely to keep while loops alive
		/// (i.e. the termination condition) and to clean them up before the application
		/// fully exits. Note: It does not terminate the program.
		/// </summary>
		public static bool Connected;
		
		/// <summary>
		/// The status of the application. If false, the application will end the
		/// current Transmission Control Protocol (TCP) connection, cleanup, and exit.
		/// </summary>
		private static bool Running = true;
		
		/// <summary>
		/// The time when the application is first started.
		/// </summary>
		public static DateTime Time;
		
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				// Add to the console
				Console.Title = "Sius";
				Console.WriteLine("Sius");
				Console.Write("Press any key to exit ..." + Environment.NewLine);
				
				Time = DateTime.Now;
				
				// Check if we are logging
				if (!string.IsNullOrEmpty(SiusConf.GetSetting.String("logging")))
					SiusLog.Logging = true;
				
				// Check if we want to clear the log before starting
				if (SiusConf.GetSetting.Boolean("clearlog"))
					SiusLog.ClearLogFile();
				
				// Create the SQLite database and tables
				SQLite.Connect();
				
				// Log that Sius has successfully started
				SiusLog.Log(SiusLog.INFORMATION, "server","Sius successfully started.");
				
				// Initialize the Transmission Control Protocol (TCP) connection
				Listen tcp = new Listen();
				tcp.StartListening();
				
				// Start the Ping timer
				System.Timers.Timer SendPing = new System.Timers.Timer();
				SendPing.Elapsed += new ElapsedEventHandler(Protocol.Ping.OnTimedEvent);
				
				if (SiusConf.GetSetting.Integer("ping") >= 10)
					SendPing.Interval = SiusConf.GetSetting.Integer("ping") * 1000;
				else
					SendPing.Interval = 150000; // 150 seconds
				
				SendPing.Enabled = true;
				SendPing.Start();
				
				// Start the Scorereset timer
				System.Timers.Timer Scorereset = new System.Timers.Timer();
				Scorereset.Elapsed += new ElapsedEventHandler(Protocol.Scorereset.OnTimedEvent);
				
				if (SiusConf.GetSetting.Integer("scorereset") >= 1)
					Scorereset.Interval = SiusConf.GetSetting.Integer("scorereset") * 3600000;
				else
					Scorereset.Interval = 1209600000; // 2 weeks
				
				Scorereset.Enabled = true;
				Scorereset.Start();
				
				// Wait until we're told to exit the program
				while (Running == true)
				{
					// press any key to exit
					Console.ReadKey(true);
					Running = false;
					SendPing.Close();
					Scorereset.Close();
				}
				
				// Cleanup and end all TCP connections
				Sius.Connected = false;
				SiusLog.Log(SiusLog.INFORMATION, "server","Exiting");
				tcp.RequestStop();
				
				// Append a line to the log to mark the end of a run.
				SiusLog.DirectLogToFile("-----------------------------------------------------");
			}
			catch (Exception e)
			{
				// Log any unexpected errors
				SiusLog.Log(SiusLog.ERROR, "server", "Error: " + e.StackTrace);
			}
		}
	}
}
