/*
 * Sius - Protocol -  Banner
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol
{
	/// <summary>
	/// Stores player banners.
	/// </summary>
	public class Banner
	{
		public static void Try(string[] parameters)
		{
			if (!String.IsNullOrEmpty(parameters[2]))
			{
				/*SQLite.SendQuery("INSERT OR REPLACE INTO 'banners' (name,banner) values ('" +
				                 pname + "','" +
				                 parameters[2] + "');");*/
				
				string name = Player.GetPlayerName(parameters[1]);
				
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"INSERT OR REPLACE INTO 'banners' (name,banner)" +
					" values (@pname,@banner)";
				SQLiteParameter pname = new SQLiteParameter("@pname");
				SQLiteParameter banner = new SQLiteParameter("@banner");
				cmd.Parameters.Add(pname);
				cmd.Parameters.Add(banner);
				pname.Value = name;
				banner.Value =  parameters[2];
				
				cmd.ExecuteNonQuery();
				
				SConnection.Close();
				SConnection.Dispose();
				/* End Database Connection */
				
				SiusLog.Log(SiusLog.INFORMATION, "banner", "Stored " + name + "'s banner into database.");
			}
		}
	}
}
