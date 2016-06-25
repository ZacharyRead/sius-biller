/*
 * Sius - Protocol - Commands - Biller Version
 */

using System;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Send the player the current biller version (assembly)
	/// </summary>
	public class Bversion
	{
		public static void Try(string[] parameters)
		{
			string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			Message.Send = "MSG:" + parameters[1] + ":0:Biller version: Sius " + version;
			SiusLog.Log(SiusLog.INFORMATION, "?bversion", "Sent biller version to " + Player.GetPlayerName(parameters[1]));
		}
	}
}
