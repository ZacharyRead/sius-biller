/*
 * Sius - Ping
 */

using System;
using System.Timers;

namespace Sius.Protocol
{
	/// <summary>
	/// Ping every zone once in a while to make sure that they're still connected.
	/// </summary>
	public class Ping
	{		
		public static void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			SiusLog.Log(SiusLog.DEBUG, "ping", "Pinged all zones to verify if they're still alive.");
			Zone.BroadcastMessage("PING");
		}
	}
}
