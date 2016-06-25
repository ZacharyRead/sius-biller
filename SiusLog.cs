/*
 * Sius - Log
 * 
 * The Sius Log Format must commence a line with the date and time
 * of the local machine enclosed between square brackets as so:
 * 		[yyyy-MM-dd HH:mm:ss]
 * 
 * It is then separated by a horizontal tab delimiter (\t).
 * 
 * The second parameter should contain short and pertinent information
 * wrapped within diamond brackets. For example, <zone>, <server>, <log>.
 * 
 * It is then separated by a horizontal tab delimiter (\t).
 * 
 * The next parameter required is the nature of the log. Like Grelminar's
 * ASSS server, it is a single character:
 * 		D = DEBUG
 * 		I = INFORMATION
 * 		M = MALICIOUS
 * 		W = WARNING
 * 		E = ERROR
 * 
 * Once again it is followed by a tab delimiter.
 * 
 * The last parameter required is the message itself. It should be as
 * detailed and informative as possible, as to avoid confusion and
 * provide a better understanding of what is happening.
 */

using System;
using System.IO;

namespace Sius
{
	/// <summary>
	/// Sius Error Logging
	/// </summary>
	public class SiusLog
	{
		public static char DEBUG = 'D';
		public static char INFORMATION = 'I';
		public static char MALICIOUS = 'M';
		public static char WARNING = 'W';
		public static char ERROR = 'E';
		
		/// <summary>
		/// Whether the current application is configured to log.
		/// True or False
		/// </summary>
		public static bool Logging = false;
		
		/// <summary>
		/// Logging stuff
		/// </summary>
		/// <param name="type"></param>
		/// <param name="message"></param>
		public static void Log(char type, string hap, string message)
		{
			if (string.IsNullOrEmpty(message))
				return;
			
			// get the current date
			string datetime = Convert.ToString(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			
			// check if we're allowing this specific log type
			if (!SiusConf.GetSetting.String("logging").Contains(Convert.ToString(type)))
				return;
			
			string tt = ">\t";
			if (hap.Length < 6)
				tt = ">\t\t";
			
			// write to screen
			Console.WriteLine(type + "\t<" + hap + tt + message);
			
			// write to file
			LogToFile("[" + datetime + "]\t" + type + "\t<" + hap + tt + message);
		}
		
		/// <summary>
		/// Logging to file
		/// </summary>
		/// <param name="message"></param>
		private static void LogToFile(string message)
		{
			if (!Logging)
				return;
			
			// create a text writer and open the file
			TextWriter tw = new StreamWriter("log.log", true);

			// append a new log message
			tw.WriteLine(message);

			// close and dispose the stream
			tw.Close();
			tw.Dispose();
		}
		
		/// <summary>
		/// Logging to file
		/// </summary>
		/// <param name="message"></param>
		public static void DirectLogToFile(string message)
		{
			if (!Logging)
				return;
			
			// create a text writer and open the file
			TextWriter tw = new StreamWriter("log.log", true);

			// append a new log message
			tw.WriteLine(message);

			// close and dispose the stream
			tw.Close();
			tw.Dispose();
		}
		
		/// <summary>
		/// Clear the log file
		/// </summary>
		public static void ClearLogFile()
		{
			// create a text writer and open the file
			TextWriter tw = new StreamWriter("log.log", false);

			// replace the content with nothing
			tw.WriteLine("");

			// close and dispose the stream
			tw.Close();
			tw.Dispose();
		}
	}
}
