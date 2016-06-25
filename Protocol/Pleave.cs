/*
 * Sius - Protocol - Player Leave
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol
{
	/// <summary>
	/// When a player leaves, remove them from the player list.
	/// </summary>
	public class Pleave
	{
		public static void Try(string[] parameters)
		{
			Player.PlayerStruct p = Player.GetPlayer(parameters[1]);
			
			/* Begin Database Connection */
			SQLiteConnection SConnection = new SQLiteConnection();
			SConnection.ConnectionString = SQLite.ConnectionString;
			SConnection.Open();
			
			SQLiteCommand cmd = new SQLiteCommand(SConnection);
			
			cmd.CommandText = @"UPDATE players SET lastseen = @lastseen WHERE name = @pname; "
				+ @"UPDATE players SET usage = (usage + @time) WHERE name = @pname;";
			SQLiteParameter lastseen = new SQLiteParameter("@lastseen");
			SQLiteParameter pname = new SQLiteParameter("@pname");
			SQLiteParameter time = new SQLiteParameter("@time");
			cmd.Parameters.Add(lastseen);
			cmd.Parameters.Add(pname);
			cmd.Parameters.Add(time);
			lastseen.Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
			pname.Value = p.name;
			
			DateTime Current = DateTime.Now;
			TimeSpan diff = Current.Subtract(p.LoginTime);
			int totalseconds = (((((diff.Days * 24) + diff.Hours) * 60) + diff.Minutes) * 60) + diff.Seconds;
			time.Value = totalseconds;
			
			cmd.ExecuteNonQuery();
			
			SConnection.Close();
			/* End Database Connection */
			
			SiusLog.Log(SiusLog.DEBUG, "leave", "Updated " + p.name + "'s player information into database.");
			
			// Remove the player
			Player.RemovePlayer(parameters[1]);
			
			/* Begin Database Connection */
			DataTable messages = new DataTable();
			
			SConnection.Open();
			cmd.CommandText = @"SELECT * FROM messages WHERE name = @pname";
			
			SQLiteDataReader Reader = cmd.ExecuteReader();
			messages.Load(Reader);
			Reader.Close();
			
			SConnection.Close();
			/* End Database Connection */
			
			if (messages.Rows.Count != 0)
			{
				for (int i = 0; i < messages.Rows.Count; i++)
				{
					if (messages.Rows[i][4].ToString() == "1")
					{
						/* Begin Database Connection */
						SConnection.Open();
						cmd.CommandText = @"DELETE FROM messages WHERE name = @pname AND sender = @sender AND message = @message";
						SQLiteParameter sender = new SQLiteParameter("@sender");
						SQLiteParameter message = new SQLiteParameter("@message");
						cmd.Parameters.Add(sender);
						cmd.Parameters.Add(message);
						sender.Value = messages.Rows[i][1].ToString();
						message.Value = messages.Rows[i][2].ToString();
						
						cmd.ExecuteNonQuery();
						
						SConnection.Close();
						/* End Database Connection */
					}
				}
				SiusLog.Log(SiusLog.INFORMATION, "leave", "Deleted " + p.name + "'s old messages.");
			}
			
			SConnection.Dispose();
			SiusLog.Log(SiusLog.INFORMATION, "leave", "Player " + p.name + " disconnected from " + Zone.ZoneName);
		}
		
		public static void oTry(string[] parameters)
		{
			//PLEAVE:pid
			string name = Player.GetPlayerName(parameters[1]);
			
			SQLite.SendQuery("UPDATE players SET lastseen = '" +
			                 DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") +
			                 "' WHERE name = '" + name + "';");
			
			// remove the player
			Player.RemovePlayer(parameters[1]);
			
			// remove the player's read messages
			System.Data.DataTable messages = SQLite.GetMultiQuery("SELECT * FROM 'messages'" +
			                                                    "WHERE name = '" + name + "';");
			if (messages.Rows.Count != 0)
			{
				for (int i = 0; i < messages.Rows.Count; i++)
				{
					if (messages.Rows[i][4].ToString() == "1")
					{
						string sender = messages.Rows[i][1].ToString();
						string message = messages.Rows[i][2].ToString();
						SQLite.SendQuery("DELETE FROM 'messages' WHERE name = '" +
						                 name + "' AND sender = '" +
						                 sender + "' AND message = '" +
						                 message + "';");
					}
				}
			}
		}
	}
}
