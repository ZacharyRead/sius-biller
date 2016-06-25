/*
 * Sius - Protocol - Commands - Password
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Change a user's password.
	/// </summary>
	public class Password
	{
		public static void Try(string[] parameters)
		{
			if (!string.IsNullOrEmpty(parameters[3]))
			{
				string name = Player.GetPlayerName(parameters[1]);
				string password = SiusUtil.Collate(3, parameters);
				
				if (password.Contains(":"))
				{
					Message.Send = "MSG:" + parameters[1] + ":0:Your password may not contain colons.";
					SiusLog.Log(SiusLog.DEBUG, "password", "Unable to change [" + name + "]'s password. Password contains a colon.");
					return;
				}
				
				//Check the password length
				if (password.Length < 1 || password.Length > 23)
				{
					Message.Send = "MSG:" + parameters[1] + ":0:The length of you password is invalid.";
					SiusLog.Log(SiusLog.DEBUG, "password", "Unable to change [" + name + "]'s password. Password length is invalid.");
					return;
				}
				
				password = SiusEncrypt.Encrypt(password, name, "players");
				
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"UPDATE 'players' SET password = @ppassword WHERE name = @pname";
				SQLiteParameter ppassword = new SQLiteParameter("@ppassword");
				SQLiteParameter pname = new SQLiteParameter("@pname");
				cmd.Parameters.Add(ppassword);
				cmd.Parameters.Add(pname);
				ppassword.Value = password;
				pname.Value = name;
				
				cmd.ExecuteNonQuery();
				
				SConnection.Close();
				SConnection.Dispose();
				/* End Database Connection */
				
				Message.Send = "MSG:" + parameters[1] + ":0:Your password has been successfully changed.";
				SiusLog.Log(SiusLog.INFORMATION, "?password", "Successfully changed " + name + "'s password.");
				SiusLog.Log(SiusLog.DEBUG, "password", "Player [" + name + "] changed password to: " + password);
			}
		}
	}
}
