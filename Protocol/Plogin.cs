/*
 * Sius - Protocol - Player Login
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol
{
	/// <summary>
	/// Handle the validity of player logins (name, password, bans...)
	/// </summary>
	public class Plogin
	{
		public static void Try(string[] parameters)
		{
			//PLOGIN:pid:flag:name:pw:ip:macid:contid
			
			// We are expecting 8 parameters (or seven occurences of ':')
			if (parameters.Length != 8)
			{
				Message.Send = "PBAD:" + parameters[1] +":Invalid player connection protocol.";
				SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3] + ") : Invalid connection protocol");
				return;
			}
			
			//Check username length
			if (parameters[3].Length < 1)
			{
				Message.Send = "PBAD:" + parameters[1] +":Username is too short.";
				SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3] + ") : Username is too short.");
				return;
			}
			
			//Check username length
			if (parameters[3].Length > 23)
			{
				Message.Send = "PBAD:" + parameters[1] +":Username is too long.";
				SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3] + ") : Username is too long.");
				return;
			}
			
			//Check password length
			if (parameters[4].Length < 1)
			{
				Message.Send = "PBAD:" + parameters[1] +":Password is too short.";
				SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3] + ") : Password is too short.");
				return;
			}
			
			//Check password length
			if (parameters[4].Length > 23)
			{
				Message.Send = "PBAD:" + parameters[1] +":Password is too long.";
				SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3] + ") : Password is too long.");
				return;
			}
			
			int newplayers = SiusConf.GetSetting.Integer("newplayers");
			
			if (newplayers > 0)
			{
				//Check if the player already exists
				
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"SELECT * FROM players WHERE name = @pname";
				SQLiteParameter pname = new SQLiteParameter("@pname");
				cmd.Parameters.Add(pname);
				pname.Value = parameters[3];
				
				SQLiteDataReader Reader = cmd.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				
				SConnection.Close();
				/* End Database Connection */
				
				//The player exists
				if (dt.Rows.Count > 0)
				{
					//Check if the password is correct
					if (SiusEncrypt.Encrypt(parameters[4], parameters[3], "players") == dt.Rows[0][2].ToString())
					{
						if (newplayers != 3)
						{
							//POK:pid:rtext:name:squad:billerid:usage:firstused
							Message.Send = "POK:" + parameters[1] + "::" + parameters[3] + ":" +
								dt.Rows[0][4].ToString() + ":" + dt.Rows[0][0].ToString() + ":" +
								dt.Rows[0][5].ToString() + ":" + dt.Rows[0][6].ToString();
							
							SiusLog.Log(SiusLog.INFORMATION, "login", parameters[3] + " successfully logged in to " + Zone.ZoneName);
							
							Player.AddPlayer(parameters[3], parameters[1], dt.Rows[0][4].ToString());
							return;
						}
						else
						{
							Message.Send = "PBAD:" + parameters[1] + ":0:The biller is currently not accepting any existing player connections.";
							SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3]
							            + ") : The biller is currently not accepting any existing player connections.");
							return;
						}
					}
					else
					{
						Message.Send = "PBAD:" + parameters[1] + ":0:Invalid password for the specified user.";
						SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3] + ") : Invalid password for the specified user.");
						return;
					}
				}
				else
				{
					//Create a new player
					if (newplayers != 2)
					{
						/* Begin Database Connection */
						SConnection.Open();
						
						//Create row
						cmd.CommandText = @"INSERT INTO players (name,created) values (@pname, @created)";
						SQLiteParameter created = new SQLiteParameter("@created");
						cmd.Parameters.Add(created);
						cmd.Parameters.Add(pname);
						created.Value = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
						pname.Value = parameters[3];
						
						cmd.ExecuteNonQuery();
						
						//Update password
						cmd.CommandText = @"UPDATE players SET password = @password where name = @pname";
						SQLiteParameter password = new SQLiteParameter("@password");
						cmd.Parameters.Add(password);
						cmd.Parameters.Add(pname);
						password.Value = SiusEncrypt.Encrypt(parameters[4], parameters[3], "players");
						pname.Value = parameters[3];
						
						cmd.ExecuteNonQuery();
						
						//Retrieve information
						cmd.CommandText = @"SELECT * FROM players WHERE name = @pname";
						cmd.Parameters.Add(pname);
						pname.Value = parameters[3];
						
						Reader = cmd.ExecuteReader();
						dt.Load(Reader);
						Reader.Close();
						
						SConnection.Close();
						SConnection.Dispose();
						/* End Database Connection */
						
						SiusLog.Log(SiusLog.INFORMATION, "user", "Succesfully created new user: " + parameters[3]);
						
						if (dt.Rows.Count > 0)
						{
							Message.Send = "POK:" + parameters[1] + "::" + parameters[3] + ":" +
								dt.Rows[0][4].ToString() + ":" + dt.Rows[0][0].ToString() + ":" +
								dt.Rows[0][5].ToString() + ":" + dt.Rows[0][6].ToString();
							
							SiusLog.Log(SiusLog.INFORMATION, "login", parameters[3] + " successfully logged in to " + Zone.ZoneName);
							
							Player.AddPlayer(parameters[3], parameters[1], dt.Rows[0][4].ToString());
							return;
						}
						else
						{
							Message.Send = "PBAD:" + parameters[1] + ":0:An error occured when attempting to log in. Please try again in a few seconds.";
							SiusLog.Log(SiusLog.ERROR, "login", "Disconnected player (" + parameters[3] + ") : Database appears to be unreachable.");
							return;
						}
						
					}
					else
					{
						Message.Send = "PBAD:" + parameters[1] + ":0:The biller is currently not accepting any new player connections.";
						SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3] 
						            + ") : The biller is currently not accepting any new player connections.");
						return;
					}
				}
			}
			else
			{
				Message.Send = "PBAD:" + parameters[1] + ":0:The biller is currently not accepting any player connections.";
				SiusLog.Log(SiusLog.WARNING, "login", "Disconnected player (" + parameters[3]
				            + ") : The biller is currently not accepting any player connections.");
				return;
			}
		}
	}
}
