/*
 * Sius - Protocol - Commands
 */

using System;

namespace Sius.Protocol
{
	/// <summary>
	/// Triage of commands.
	/// </summary>
	public class Cmd
	{
		public static void Try(string[] parameters)
		{
			//CMD:pid:cmdname:args
			string command = parameters[2].ToLower();
			
			//TODO: Check permission
			SiusLog.Log(SiusLog.DEBUG, "command", "Received: ?" + command);
			
			if (command == "chat")
				Commands.Chat.Try(parameters);
			else if (command == "find")
				Commands.Find.Try(parameters);
			else if (command == "squad")
				Commands.Squad.Try(parameters);
			else if (command == "squadowner")
				Commands.Squadowner.Try(parameters);
			else if (command == "squadlist")
				Commands.Squadlist.Try(parameters);
			else if (command == "squadcreate")
				Commands.Squadcreate.Try(parameters);
			else if (command == "squadleave")
				Commands.Squadleave.Try(parameters);
			else if (command == "squadjoin")
				Commands.Squadjoin.Try(parameters);
			else if (command == "squadkick")
				Commands.Squadkick.Try(parameters);
			else if (command == "password")
				Commands.Password.Try(parameters);
			else if (command == "message" || command == "messages")
				Commands.Messages.Try(parameters);
			else if (command == "bzones" || command == "zones")
				Commands.Bzones.Try(parameters);
			else if (command == "bzone")
				Commands.Bzone.Try(parameters);
			else if (command == "userid")
				Commands.UserID.Try(parameters);
			else if (command == "squadpassword")
				Commands.Squadpassword.Try(parameters);
			else if (command == "btime")
				Commands.Btime.Try(parameters);
			else if (command == "buptime")
				Commands.Buptime.Try(parameters);
			else if (command == "bversion")
				Commands.Bversion.Try(parameters);
			else if (command == "squaddissolve")
				Commands.Squaddissolve.Try(parameters);
			else if (command == "squadgrant")
				Commands.Squadgrant.Try(parameters);
		}
	}
}
