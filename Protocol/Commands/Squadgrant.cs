/*
 * Sius - Protocol - Commands - Squad Grant
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Transfer the ownership of a squad to another player
	/// </summary>
	public class Squadgrant
	{
		public static void Try(string[] parameters)
		{
			Player.PlayerStruct p = Player.GetPlayer(parameters[1]);
			if (!string.IsNullOrEmpty(p.Squad) && !string.IsNullOrEmpty(parameters[3]))
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
						DataTable check = new DataTable();
						SConnection.Open();
						cmd.CommandText = @"SELECT squad FROM players WHERE name = @gplayer";
						SQLiteParameter gplayer = new SQLiteParameter("@gplayer");
						cmd.Parameters.Add(gplayer);
						gplayer.Value = parameters[3];
						
						SQLiteDataReader Reade = cmd.ExecuteReader();
						check.Load(Reade);
						Reade.Close();
						
						SConnection.Close();
						/* End Database Connection */
						
						if (check.Rows.Count > 0)
						{
							if (check.Rows[0][0].ToString().ToLower() == p.Squad.ToLower())
							{
								SConnection.Open();
								cmd.CommandText = @"UPDATE squads SET owner = @gplayer WHERE name = @nsquad";
								cmd.ExecuteNonQuery();
								SConnection.Close();
								SConnection.Dispose();
								
								Message.Send = "MSG:" + parameters[1] + ":0:Ownership of the squad has been granted to " + parameters[3] + ".";
								return;
							}
							else
								Message.Send = "MSG:" + parameters[1] + ":0:This player is not on your squad.";
						}
						else
							Message.Send = "MSG:" + parameters[1] + ":0:This player does not exist.";
					}
					else
					{
						Message.Send = "MSG:" + parameters[1] + ":0:You do not own this squad.";
						return;
					}
				}
			}
		}
	}
}
