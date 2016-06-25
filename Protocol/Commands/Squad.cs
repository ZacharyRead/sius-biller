/*
 * Sius - Protocol - Commands - Squad
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Send the player the name of his own squad, or that of the specified player.
	/// </summary>
	public class Squad
	{
		public static void Try(string[] parameters)
		{
			string name = Player.GetPlayerName(parameters[1]);
			
			if (string.IsNullOrEmpty(parameters[3])) //display self squad
			{
				string squad = Player.GetPlayer(parameters[1]).Squad;
				if (string.IsNullOrEmpty(squad))
					Message.Send = "MSG:" + parameters[1] + ":0:You are not currently on any squad.";
				else
					Message.Send = "MSG:" + parameters[1] + ":0:Squad: " + squad;
				
				SiusLog.Log(SiusLog.INFORMATION, "?squad", "Sent " + name + " his or her squad name.");
			}
			else //display other person's squad
			{
				string squad = Player.GetSquad(parameters[3]);
				if (!string.IsNullOrEmpty(squad))
				{
					Message.Send = "MSG:" + parameters[1] + ":0:Squad: " + squad;
					SiusLog.Log(SiusLog.DEBUG, "?squad", "Sent <" + name + "> " + parameters[3]
					            + "'s squad name (" + squad + ").");
				}
				else
				{
					if (Player.Exists(parameters[3]))
						Message.Send = "MSG:" + parameters[1] + ":0:This player is not on any squads.";
					else
						Message.Send = "MSG:" + parameters[1] + ":0:Unknown player";
					
					SiusLog.Log(SiusLog.DEBUG, "?squad", "Attempted to send <" + name + "> " + parameters[3]
					            + "'s squad name, but this player either doesn't exist or is not currently on any squads.");
				}
			}
		}
	}
}
