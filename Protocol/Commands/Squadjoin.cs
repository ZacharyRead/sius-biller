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
	/// Add a player to a squad if the password is correct.
	/// </summary>
	public class Squadjoin
	{
		public static void Try(string[] parameters)
		{
			if (!string.IsNullOrEmpty(parameters[3]))
			{
				string squad = parameters[3];
				string password = SiusUtil.Collate(4, parameters);
				
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"SELECT * FROM squads WHERE name = @nsquad";
				SQLiteParameter nsquad = new SQLiteParameter("@nsquad");
				cmd.Parameters.Add(nsquad);
				nsquad.Value = squad;
				
				SQLiteDataReader Reader = cmd.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				
				SConnection.Close();
				/* End Database Connection */
				
				//The squad exists
				if (dt.Rows.Count > 0)
				{
					//Check if the password is correct
					if (SiusEncrypt.Encrypt(password, squad, "squads") == dt.Rows[0][1].ToString())
					{
						SConnection.Open();
						cmd.CommandText = @"UPDATE players SET squad = @nsquad WHERE name = @pname";
						SQLiteParameter pname = new SQLiteParameter("@pname");
						cmd.Parameters.Add(pname);
						pname.Value = Player.GetPlayerName(parameters[1]);
						
						cmd.ExecuteNonQuery();
						
						SConnection.Close();
						SConnection.Dispose();
						
						Message.Send = "MSG:" + parameters[1] + ":0:Your squad has been successfully changed."
							+ " Please log back in for the change to take effect.";
						
						SiusLog.Log(SiusLog.INFORMATION, "?squadjoin", Player.GetPlayerName(parameters[1])
						            + " successfully joined squad <" + squad + ">.");
						
						return;
					}
					else
					{
						Message.Send = "MSG:" + parameters[1] + ":0:Invalid password for the specified squad.";
						SiusLog.Log(SiusLog.DEBUG, "?squadjoin", Player.GetPlayerName(parameters[1])
						            + " was unable to join squad <" + squad + "> (invalid password).");
					}
				}
				else
				{
					Message.Send = "MSG:" + parameters[1] + ":0:This squad does not exist.";
					SiusLog.Log(SiusLog.DEBUG, "?squadjoin", Player.GetPlayerName(parameters[1])
						            + " was unable to join squad <" + squad + "> (squad does not exist).");
				}
			}
		}
	}
}
