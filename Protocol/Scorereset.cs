/*
 * Sius - Biller-wide Zone Score Reset
 */

using System;
using System.Timers;

namespace Sius.Protocol
{
	/// <summary>
	/// After a certain interval, resets the score of each zone.
	/// </summary>
	public class Scorereset
	{
		public static void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			Zone.BroadcastMessage("SCORERESET");
			SiusLog.Log(SiusLog.INFORMATION, "score", "Sent score reset to all connected zones.");
		}
	}
}
