/*
 * Sius - Protocol - Commands - Bzone
 */

using System;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Send the name of the current zone that the player is in.
	/// </summary>
	public class Bzone
	{
		public static void Try(string[] parameters)
		{
			Message.Send = "MSG:" + parameters[1] + ":0:Zone: " + Zone.ZoneName;
			SiusLog.Log(SiusLog.INFORMATION, "?bzone", "Sent network zone name to " + Player.GetPlayerName(parameters[1]));
		}
	}
}
