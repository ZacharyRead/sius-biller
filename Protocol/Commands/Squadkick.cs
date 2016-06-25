/*
 * Sius - Protocol - Commands - Squad Kick
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Kick a player from squad.
	/// </summary>
	public class Squadkick
	{
		public static void Try(string[] parameters)
		{
			Player.PlayerStruct p = Player.GetPlayer(parameters[1]);
			if (!string.IsNullOrEmpty(p.Squad) && !string.IsNullOrEmpty(parameters[3]) && p.name.ToLower() != parameters[3].ToLower())
			{
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
			
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"SELECT owner FROM squads WHERE name = @nsquad";
				SQLiteParameter nsquad = new SQLiteParameter("@nsquad");
				SQLiteParameter pname = new SQLiteParameter("@pname");
				cmd.Parameters.Add(nsquad);
				cmd.Parameters.Add(pname);
				nsquad.Value = p.Squad;
				pname.Value = p.name;
				
				SQLiteDataReader Reader = cmd.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				
				SConnection.Close();
				/* End Database Connection */
				
				if (dt.Rows.Count > 0)
				{
					if (dt.Rows[0][0].ToString().ToLower() == p.name.ToLower())
					{
						/* Begin Database Connection */
						SConnection.Open();
						cmd.CommandText = @"UPDATE players SET squad = '' WHERE name = @kname AND squad = @nsquad";
						SQLiteParameter kname = new SQLiteParameter("@kname");
						cmd.Parameters.Add(kname);
						kname.Value = parameters[3];
						cmd.ExecuteNonQuery();
						SConnection.Close();
						/* End Database Connection */
						
						//TODO: do we send the kicked player a message?
						Message.Send = "MSG:" + parameters[1] + ":0:If said player was a member of your squad, he or she has been kicked.";
						SiusLog.Log(SiusLog.INFORMATION, "?squadkick", p.name + " kicked " + parameters[3] + " from squad <" + p.Squad + ">.");
					}
					else
					{
						Message.Send = "MSG:" + parameters[1] + ":0:You do not own this squad.";
						SiusLog.Log(SiusLog.INFORMATION, "?squadkick", p.name + " attempted to kick " + parameters[3] + " from squad <"
						            + p.Squad + ">, but is not owner.");
						return;
					}
				}
			}
		}
	}
}
