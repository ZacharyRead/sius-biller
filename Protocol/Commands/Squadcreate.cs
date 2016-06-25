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
	/// Create a squad if it does not exist.
	/// </summary>
	public class Squadcreate
	{
		public static void Try(string[] parameters)
		{
			if (!string.IsNullOrEmpty(parameters[3]))
			{
				string squad = parameters[3];
				string password = SiusUtil.Collate(4, parameters);
				string name = Player.GetPlayerName(parameters[1]);
				
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
					Message.Send = "MSG:" + parameters[1] + ":0:Sorry, this squad already exists.";
					SiusLog.Log(SiusLog.DEBUG, "?squadcreate", "Unable to create squad <" + squad + "> for " + name
					           + ", but squad already exists.");
					SConnection.Dispose();
					return;
				}
				else
				{
					SConnection.Open();
					
					cmd.CommandText = @"INSERT INTO squads (name, owner, lastact) values (@nsquad, @pname, @last)";
					SQLiteParameter last = new SQLiteParameter("@last");
					SQLiteParameter pname = new SQLiteParameter("@pname");
					cmd.Parameters.Add(last);
					cmd.Parameters.Add(pname);
					last.Value = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
					pname.Value = name;
					cmd.ExecuteNonQuery();
					
					cmd.CommandText = @"UPDATE squads SET password = @spass WHERE name = @nsquad";
					SQLiteParameter spass = new SQLiteParameter("@spass");
					cmd.Parameters.Add(spass);
					spass.Value = SiusEncrypt.Encrypt(password, squad, "squads");
					cmd.ExecuteNonQuery();
					
					cmd.CommandText = @"UPDATE players SET squad = @nsquad WHERE name = @pname";
					cmd.ExecuteNonQuery();
					
					SConnection.Close();
					SConnection.Dispose();
					
					Message.Send = "MSG:" + parameters[1] + ":0:Squad successfully created. Please exit"
						+ " your current zone and log back in for the change to take effect.";
					
					SiusLog.Log(SiusLog.INFORMATION, "?squadcreate", "Successfully created squad <" + squad + "> for " + name);
					return;
				}
			}
		}
	}
}
