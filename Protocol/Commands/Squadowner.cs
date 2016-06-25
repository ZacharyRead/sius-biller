/*
 * Sius - Protocol - Commands - Squadowner
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Send the player the name of the squad's owner.
	/// </summary>
	public class Squadowner
	{
		public static void Try(string[] parameters)
		{
			if (!string.IsNullOrEmpty(parameters[3]))
			{
				/*Message.Send = "MSG:" + parameters[1] + ":0:Owner is " +
					SQLite.GetQuery("SELECT owner FROM 'squads' WHERE name = '" +
					                parameters[3] + "';");*/
				
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"SELECT owner FROM 'squads' WHERE name = @pname";
				SQLiteParameter pname = new SQLiteParameter("@pname");
				cmd.Parameters.Add(pname);
				pname.Value = parameters[3];
				
				SQLiteDataReader Reader = cmd.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				
				SConnection.Close();
				/* End Database Connection */
				
				if (dt.Rows.Count > 0)
				{
					Message.Send = "MSG:" + parameters[1] + ":0:Owner is " + dt.Rows[0][0].ToString();
					SiusLog.Log(SiusLog.INFORMATION, "?squadowner", Player.GetPlayerName(parameters[1])
					            + " requested the squad owner of " + parameters[3]);
				}
			}
		}
	}
}
