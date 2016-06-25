/*
 * Sius - Protocol - Commands - Squadjoin
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Description of Squadleave.
	/// </summary>
	public class Squadleave
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
					DataTable others = new DataTable();
					SConnection.Open();
					cmd.CommandText = @"SELECT * FROM players WHERE squad = @nsquad";
					
					SQLiteDataReader Reade = cmd.ExecuteReader();
					others.Load(Reade);
					Reade.Close();
					SConnection.Close();
					
					if (others.Rows.Count > 1) //there are others on this squad
					{
						if (dt.Rows[0][0].ToString().ToLower() == p.name.ToLower()) //is owner
						{
							Message.Send = "MSG:" + parameters[1] + ":0:Unable to remove you from squad. Consider transfering ownership"
								+ " to someone else (?squadgrant <name>), or dissolving this squad (?squaddissolve).";
							
							SiusLog.Log(SiusLog.INFORMATION, "?squadleave", "Unable to remove " + p.name + " from squad <" + p.Squad + "> due to ownership.");
							return;
							
							//cmd.CommandText = @"UPDATE squads SET owner = '**UNKNOWN**' WHERE name = @nsquad"; //TODO: Should we allow him to squadleave?
							//cmd.ExecuteNonQuery();
						}
						SConnection.Open();
						cmd.CommandText = @"UPDATE players SET squad = '' WHERE name = @pname";
						cmd.ExecuteNonQuery();
						SConnection.Close();
						
						Message.Send = "MSG:" + parameters[1] + ":0:Left squad. Please exit the zone for the changes to take effect.";
						SiusLog.Log(SiusLog.INFORMATION, "?squadleave", p.name + " left squad <" + p.Squad + ">");
					}
					else //player is lone on squad
					{
						SConnection.Open();
						cmd.CommandText = @"DELETE FROM squads WHERE name = @nsquad";
						cmd.ExecuteNonQuery();
						cmd.CommandText = @"UPDATE players SET squad = '' WHERE name = @pname";
						cmd.ExecuteNonQuery();
						SConnection.Close();
						
						Message.Send = "MSG:" + parameters[1] + ":0:Left squad. Please exit the zone for the changes to take effect.";
						SiusLog.Log(SiusLog.INFORMATION, "?squadleave", p.name + " left squad <" + p.Squad + ">");
						SiusLog.Log(SiusLog.DEBUG, "?squadleave", "Deleted squad from database: no other players.");
					}
					SConnection.Dispose();
					return;
				}
				else
				{
					//squad doesn't exist? no owner? Remove player from squad only.
					SiusLog.Log(SiusLog.WARNING,"squad","Unable to locate the squad <" + p.Squad + "> in database.");
					
					SConnection.Open();
					cmd.CommandText = @"UPDATE players SET squad = '' WHERE name = @pname";
					cmd.ExecuteNonQuery();
					SConnection.Close();
					SConnection.Dispose();
						
					Message.Send = "MSG:" + parameters[1] + ":0:Left squad. Please exit the zone for the changes to take effect.";
					SiusLog.Log(SiusLog.INFORMATION, "?squadleave", p.name + " left squad <" + p.Squad + ">");
					
					return;
				}
			}
		}
	}
}
