/*
 * Sius - Protocol - Commands - User Identification
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Send a player is user identification number.
	/// </summary>
	public class UserID
	{
		public static void Try(string[] parameters)
		{
			string name = Player.GetPlayerName(parameters[1]);
			
			/* Begin Database Connection */
			DataTable dt = new DataTable();
		
			SQLiteConnection SConnection = new SQLiteConnection();
			SConnection.ConnectionString = SQLite.ConnectionString;
			SConnection.Open();
			
			SQLiteCommand cmd = new SQLiteCommand(SConnection);
			
			cmd.CommandText = @"SELECT id FROM 'players' WHERE name = @pname";
			SQLiteParameter pname = new SQLiteParameter("@pname");
			cmd.Parameters.Add(pname);
			pname.Value = name;
			
			SQLiteDataReader Reader = cmd.ExecuteReader();
			dt.Load(Reader);
			Reader.Close();
			
			SConnection.Close();
			SConnection.Dispose();
			/* End Database Connection */
			
			if (dt.Rows.Count > 0)
			{
				string userid = dt.Rows[0][0].ToString();
				Message.Send = "MSG:" + parameters[1] + ":0:UserID: " + userid;
				SiusLog.Log(SiusLog.INFORMATION, "?userid", "Player [" + name + "] requested userid: " + userid);
			}
			else
			{
				Message.Send = "MSG:" + parameters[1] + ":0:An error occured while trying to retrieve your userid.";
				SiusLog.Log(SiusLog.DEBUG, "?userid", "An error occured while trying to retrieve " + name + "'s userid");
			}
		}
	}
}
