/*
 * Sius - Protocol - Commands - Find
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Find a player and report whether they exist or not, what zone they're currently in, or
	/// how long ago they were last online.
	/// </summary>
	public class Find
	{
		public static void Try(string[] parameters)
		{
			//CMD:pid:find:name
			for (int i = 0; i < Player.PlayerList.Count; i++)
	    	{
	    		Player.PlayerStruct p = (Player.PlayerStruct)Player.PlayerList[i];
	    		
	    		if (p.name == parameters[3])
	    		{
	    			Message.Send = "MSG:" + parameters[1] + ":0:" + p.name + " is in " + p.ZoneName;
	    			SiusLog.Log(SiusLog.INFORMATION, "?find", "Located " + parameters[3] + " for " + Player.GetPlayerName(parameters[1]));
	    			return;
	    		}
	    	}
			
			/* Begin Database Connection */
			DataTable dt = new DataTable();
			
			SQLiteConnection SConnection = new SQLiteConnection();
			SConnection.ConnectionString = SQLite.ConnectionString;
			SConnection.Open();
			
			SQLiteCommand cmd = new SQLiteCommand(SConnection);
			
			cmd.CommandText = @"SELECT lastseen FROM 'players' WHERE name = @pname";
			SQLiteParameter pname = new SQLiteParameter("@pname");
			cmd.Parameters.Add(pname);
			pname.Value = parameters[3];
			
			SQLiteDataReader Reader = cmd.ExecuteReader();
			dt.Load(Reader);
			Reader.Close();
			
			SConnection.Close();
			SConnection.Dispose();
			/* End Database Connection */
			
			if (dt.Rows.Count > 0)
			{
				DateTime Seen = new DateTime();
				Seen = DateTime.ParseExact(dt.Rows[0][0].ToString(), "yyyy-MM-dd HH:mm:ss",
				                           System.Globalization.CultureInfo.InvariantCulture);
				
				TimeSpan diff = DateTime.UtcNow.Subtract(Seen);
				if (diff.Days >= 10)
				{
					Message.Send = "MSG:" + parameters[1] + ":0:Not online, last seen over 10 days ago.";
				}
				else if (diff.Days >= 1)
				{
					Message.Send = "MSG:" + parameters[1] + ":0:Not online, last seen " + diff.Days +
						(diff.Days == 1 ? " day" : " days") + " ago";
				}
				else if (diff.Hours >= 1)
				{
					Message.Send = "MSG:" + parameters[1] + ":0:Not online, last seen" + diff.Hours +
						(diff.Hours == 1 ? " hour" : " hours") + " ago";
				}
				else
				{
					Message.Send = "MSG:" + parameters[1] + ":0:Not online, last seen " + diff.Minutes +
						(diff.Minutes == 1 ? " minute" : " minutes") + " ago";
				}
			}
			else
			{
				Message.Send = "MSG:" + parameters[1] + ":0:Unknown user.";
			}
			SiusLog.Log(SiusLog.INFORMATION, "?find", "Attempted to locate when " + parameters[3] + " was last seen for " + Player.GetPlayerName(parameters[1]));
		}
	}
}
