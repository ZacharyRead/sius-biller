/*
 * Sius - Protocol - Commands - Squad Password
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// If owner, change the password of squad.
	/// </summary>
	public class Squadpassword
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
					if (dt.Rows[0][0].ToString().ToLower() == p.name.ToLower()) //if owner
					{
						string password = SiusUtil.Collate(3, parameters);
						if (string.IsNullOrEmpty(password))
						{
							Message.Send = "MSG:" + parameters[1] + ":0:Please specify a password.";
							return;
						}
						if (password.Length > 23)
						{
							Message.Send = "MSG:" + parameters[1] + ":0:Password is too long.";
							return;
						}
						
						password = SiusEncrypt.Encrypt(password, p.Squad, "squads");
						
						/* Begin Database Connection */
						SConnection.Open();
						cmd.CommandText = @"UPDATE squads SET password = @spass WHERE name = @nsquad";
						SQLiteParameter spass = new SQLiteParameter("@spass");
						cmd.Parameters.Add(spass);
						spass.Value = password;
						cmd.ExecuteNonQuery();
						SConnection.Close();
						/* End Database Connection */
						
						Message.Send = "MSG:" + parameters[1] + ":0:Squad password successfully changed.";
						SiusLog.Log(SiusLog.INFORMATION, "?squadpassword", p.name + " successfully changed the password to squad <" + p.Squad + ">");
						return;
					}
					else
					{
						Message.Send = "MSG:" + parameters[1] + ":0:You do not own this squad.";
						SiusLog.Log(SiusLog.INFORMATION, "?squadpassword", p.name + " attempted to change the password of <"
						            + p.Squad + ">, but is not owner.");
						return;
					}
				}
			}
			Message.Send = "MSG:" + parameters[1] + ":0:You are not on a squad.";
		}
	}
}
