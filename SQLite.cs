/*
 * Sius - SQLite
 * 
 * Creates a small database (users.siu) that stores player information.
 * 
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius
{
	/// <summary>
	/// Description of SQLite.
	/// </summary>
	public class SQLite
	{
		public static string ConnectionString = "Data Source=users.siu;UseUTF16Encoding=True;Legacy Format=False;";
		
		public static void Connect()
		{
			try
			{
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString =
					"Data Source=users.siu;"  +
					"UseUTF16Encoding=True;"  +
					"Legacy Format=False;";
				
				SConnection.Open();
				
				CreatePlayersTable(SConnection);
				CreateBannersTable(SConnection);
				CreateMessagesTable(SConnection);
				CreateSquadsTable(SConnection);
				SiusLog.Log(SiusLog.DEBUG, "SQLite", "Attempted to create all necessary database tables.");
				
				SConnection.Close();
				SConnection.Dispose();
			}
			catch (Exception e)
			{
				SiusLog.Log(SiusLog.ERROR, "SQLite", e.Message);
			}
		}
		
		/// <summary>
		/// Creates the initial "players" table.
		/// ID - NAME - PASSWORD - USAGE - CREATED - LASTLOGIN
		/// </summary>
		private static void CreatePlayersTable(SQLiteConnection SConnection)
		{
			try
			{
				SQLiteCommand Command = new SQLiteCommand(SConnection);
				Command.CommandText =
					"CREATE TABLE IF NOT EXISTS [players] (" +
					"[id] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL," +
					"[name] TEXT COLLATE NOCASE  UNIQUE NOT NULL," +
					"[password] TEXT  NULL," +
					"[salt] TEXT  NULL," +
					"[squad] TEXT COLLATE NOCASE  NULL," +
					"[usage] INTEGER  DEFAULT 0," +
					"[created] TEXT  NULL," +
					"[lastseen] TEXT DEFAULT CURRENT_TIMESTAMP NULL" +
					")";
				
				Command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				SiusLog.Log(SiusLog.ERROR, "SQLite", e.Message);
			}
		}
		
		/// <summary>
		/// Creates the initial "banners" table.
		/// ID - NAME - BANNER
		/// </summary>
		private static void CreateBannersTable(SQLiteConnection SConnection)
		{
			try
			{
				SQLiteCommand Command = new SQLiteCommand(SConnection);
				Command.CommandText =
					"CREATE TABLE IF NOT EXISTS [banners] (" +
					"[name] TEXT COLLATE NOCASE  UNIQUE NOT NULL," +
					"[banner] TEXT  NOT NULL" +
					")";
				
				Command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				SiusLog.Log(SiusLog.ERROR, "SQLite", e.Message);
			}
		}
		
		/// <summary>
		/// Creates the initial "messages" table.
		/// ID - NAME - MESSAGE - FLAG
		/// </summary>
		private static void CreateMessagesTable(SQLiteConnection SConnection)
		{
			try
			{
				SQLiteCommand Command = new SQLiteCommand(SConnection);
				Command.CommandText =
					"CREATE TABLE IF NOT EXISTS [messages] (" +
					"[name] TEXT COLLATE NOCASE  NOT NULL," +
					"[sender] TEXT COLLATE NOCASE  NOT NULL," +
					"[message] TEXT  NOT NULL," +
					"[time] TEXT  NULL," +
					"[flag] INTEGER  DEFAULT 0" +
					")";
				
				Command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				SiusLog.Log(SiusLog.ERROR, "SQLite", e.Message);
			}
		}
		
		/// <summary>
		/// Creates the initial "squads" table.
		/// ID - NAME - BANNER
		/// </summary>
		private static void CreateSquadsTable(SQLiteConnection SConnection)
		{
			try
			{
				SQLiteCommand Command = new SQLiteCommand(SConnection);
				Command.CommandText =
					"CREATE TABLE IF NOT EXISTS [squads] (" +
					"[name] TEXT COLLATE NOCASE  UNIQUE NOT NULL," +
					"[password] TEXT  NULL," +
					"[salt] TEXT  NULL," +
					"[owner] TEXT COLLATE NOCASE NOT NULL," +
					"[lastact] TEXT  NOT NULL" +
					")";
				
				Command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				SiusLog.Log(SiusLog.ERROR, "SQLite", e.Message);
			}
		}
		
		/// <summary>
		/// Will send a sql query without expecting a result returned.
		/// </summary>
		/// <param name="sql"></param>
		public static void SendQuery(string sql)
		{
			try
			{
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString =
					"Data Source=users.siu;UseUTF16Encoding=True;Legacy Format=False;";
				
				SConnection.Open();
				
				SQLiteCommand Command = new SQLiteCommand(SConnection);
				Command.CommandText = sql;
				Command.ExecuteNonQuery();
				Command.Dispose();
				SConnection.Close();
				SConnection.Dispose();
			}
			catch (Exception e)
			{
				SiusLog.Log(SiusLog.DEBUG, "SendQuery", e.Message);
			}
		}
		
		/// <summary>
		/// Queries the database, returns a single string.
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static string GetQuery(string sql)
		{
			DataTable dt = new DataTable();
			
			try
			{
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString =
					"Data Source=users.siu;"  +
					"UseUTF16Encoding=True;"  +
					"Legacy Format=False;";
				
				SConnection.Open();
				
				SQLiteCommand Command = new SQLiteCommand(SConnection);
				Command.CommandText = sql;
				
				SQLiteDataReader Reader = Command.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				Reader.Dispose();
				Command.Dispose();
				
				SConnection.Close();
				
				return dt.Rows[0][0].ToString();
			}
			catch (Exception e)
			{
				SiusLog.Log(SiusLog.WARNING, "GetQuery", e.Message);
				return "";
			}
		}
		
		/// <summary>
		/// Queries the database, returns an datatable.
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static DataTable GetMultiQuery(string sql)
		{
			DataTable dt = new DataTable();
			
			try
			{
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString =
					"Data Source=users.siu;"  +
					"UseUTF16Encoding=True;"  +
					"Legacy Format=False;";
				
				SConnection.Open();
				
				SQLiteCommand Command = new SQLiteCommand(SConnection);
				Command.CommandText = sql;
				
				SQLiteDataReader Reader = Command.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				Reader.Dispose();
				Command.Dispose();
				
				SConnection.Close();
				SConnection.Dispose();
			}
			catch (Exception e)
			{
				SiusLog.Log(SiusLog.WARNING, "GetQuery", e.Message);
			}
			return dt;
		}
	}
}
