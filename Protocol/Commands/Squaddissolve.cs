/*
 * Sius - Protocol - Commands - Squad Dissolve
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Dissolves the current squad and removes all players enlisted.
	/// </summary>
	public class Squaddissolve
	{
		public static void Try(string[] parameters)
		{
			Player.PlayerStruct p = Player.GetPlayer(parameters[1]);
			if (!string.IsNullOrEmpty(p.Squad))
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
						cmd.CommandText = @"DELETE FROM squads WHERE name = @nsquad AND owner = @pname";
						cmd.ExecuteNonQuery();
						
						cmd.CommandText = @"UPDATE players SET squad = '' WHERE squad = @nsquad";
						cmd.ExecuteNonQuery();
						SConnection.Close();
						/* End Database Connection */
						
						Message.Send = "MSG:" + parameters[1] + ":0:Squad dissolved.";
						SiusLog.Log(SiusLog.DEBUG, "?squaddissolve", "Dissolved squad <" + p.Squad
						            + "> for " + p.name);
					}
					else
					{
						Message.Send = "MSG:" + parameters[1] + ":0:You do not own this squad.";
						SiusLog.Log(SiusLog.DEBUG, "?squaddissolve", "Unable to dissolve squad <" + p.Squad
						            + "> for " + p.name + " (not owner).");
						return;
					}
				}
			}
		}
	}
}
