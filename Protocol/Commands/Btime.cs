/*
 * Sius - Protocol - Commands - Biller Time
 */

using System;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Description of Btime.
	/// </summary>
	public class Btime
	{
		public static void Try(string[] parameters)
		{
			Message.Send = "MSG:" + parameters[1] + ":0:Current biller date and time: " +
				System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (UTC: " +
				System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + ")";
			
			SiusLog.Log(SiusLog.INFORMATION, "?btime", "Sent biller local date and time to " + Player.GetPlayerName(parameters[1]));
		}
	}
}
