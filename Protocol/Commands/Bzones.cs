/*
 * Sius - Protocol - Commands - Bzones
 */

using System;
using System.Collections;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Send the player a list of all the zones currently connected to the biller.
	/// </summary>
	public class Bzones
	{
		public static void Try(string[] parameters)
		{
			string zones = "";
			
			string[] ZoneList = new string[Listen.htZones.Count];
			Listen.htZones.Values.CopyTo(ZoneList, 0);
			
			for (int i = 0; i < ZoneList.Length; i++)
			{
				if (zones == "")
					zones += ZoneList[i].ToString() + " (" + Zone.PidToName.Count.ToString() + ")";
				else
					zones += ", " + ZoneList[i].ToString() + " (" + Zone.PidToName.Count.ToString() + ")";
			}
			
			Message.Send = "MSG:" + parameters[1] + ":0:" + zones;
			SiusLog.Log(SiusLog.INFORMATION, "?bzones", "Sent network zone list to " + Player.GetPlayerName(parameters[1]));
		}
	}
}
