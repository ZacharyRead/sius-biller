/*
 * Sius - Protocol
 */

using System;

namespace Sius
{
	/// <summary>
	/// Description of Protocol.
	/// </summary>
	public class Message
	{
		// raw protocol separated into a string array
		[ThreadStatic]
		static string[] parameters;
		
		// what we send back to the zone
		[ThreadStatic]
		public static string Send;
		
		/// <summary>
		/// Handle incoming messages from the TCP client and separate all parameters.
		/// </summary>
		/// <param name="message"></param>
		public static string Receive(string message)
		{
			Send = ""; //reset
			
			parameters = message.Split(':');
			
			First(parameters[0]);
			
			return Send;
		}
		
		/// <summary>
		/// Sort out the incoming message type based on the first parameter of the string.
		/// </summary>
		/// <param name="param"></param>
		private static void First(string param)
		{
			switch (param)
			{
				case "CONNECT" :
					Message.Send = "CONNECTBAD:Sius:" + SiusConf.GetSetting.String("network") +
						":Invalid connection attempt.";
					break;
				case "PLOGIN" :
					Protocol.Plogin.Try(parameters);
					break;
				case "REGDATA" :
					//Protocol.Regdata(parameters);
					break;
				case "BNR" :
					Protocol.Banner.Try(parameters);
					break;
				case "PENTERARENA" :
					Protocol.Penterarena.Try(parameters);
					break;
				case "PLEAVE" :
					Protocol.Pleave.Try(parameters);
					break;
				case "CHAT" :
					Protocol.Chat.Try(parameters);
					break;
				case "RMT" :
					Protocol.Rmt.Try(parameters);
					break;
				case "RMTSQD" :
					Protocol.Rmtsqd.Try(parameters);
					break;
				case "CMD" :
					Protocol.Cmd.Try(parameters);
					break;
				case "LOG" :
					Protocol.Log.Try(parameters);
					break;
				default :
					break;
			}
			return;
		}
	}
}
