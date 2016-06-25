/*
 * Sius - Protocol - Commands - Biller Uptime
 */

using System;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Send the player the amount of time that the biller has been active.
	/// </summary>
	public class Buptime
	{
		public static void Try(string[] parameters)
		{
			DateTime Current = DateTime.Now;
			TimeSpan diff = Current.Subtract(Sius.Time);
			Message.Send = "MSG:" + parameters[1] + ":0:Biller Uptime: " +
				diff.Seconds.ToString() + (diff.Seconds == 1 ? " second " : " seconds ") +
				diff.Minutes.ToString() + (diff.Minutes == 1 ? " minute " : " minutes ") +
				diff.Hours.ToString() + (diff.Hours == 1 ? " hour " : " hours ") +
				diff.Days.ToString() + (diff.Days == 1 ? " day" : " days");
			
			SiusLog.Log(SiusLog.INFORMATION, "?buptime", "Sent biller uptime to " + Player.GetPlayerName(parameters[1]));
		}
	}
}
