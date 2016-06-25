/*
 * Sius - Protocol - Remote Squad Messaging
 */

using System;

namespace Sius.Protocol
{
	/// <summary>
	/// Send a message to all online members of a squad.
	/// </summary>
	public class Rmtsqd
	{
		//RMTSQD:pid:destsquad:sound:text
		public static void Try(string[] parameters)
		{
			if (string.IsNullOrEmpty(parameters[4]) || string.IsNullOrEmpty(parameters[2]))
				return;
			
			string squad = parameters[2].ToLower();
			string name = Player.GetPlayerName(parameters[1]);
			string message = SiusUtil.Collate(4, parameters);
			
			if (SiusConf.GetSetting.Boolean("squadonlychat"))
			{
				//player is sending a squad message to a different squad
				if (Player.GetSquad(parameters[1]).ToLower() != squad)
				{
					SiusLog.Log(SiusLog.WARNING, "squad", name
					            + " tried to send a message to a different squad (refer to settings - squadonlychat).");
					return;
				}
			}
			
			//It's easier to blindly send the message
			//RMTSQD:destsquad:sender:sound:text
			Zone.BroadcastMessage("RMTSQD:" + squad + ":" + name + ":" + parameters[3] + ":" + message);
			
			SiusLog.Log(SiusLog.INFORMATION, "squad", name + " sent squad message to " + squad);
			SiusLog.Log(SiusLog.DEBUG, "squad", "Message: " + message);
		}
	}
}
