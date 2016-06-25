/*
 * Sius - Player
 */

using System;
using System.Collections;
using System.Net.Sockets;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius
{
	/// <summary>
	/// Player
	/// </summary>
	public class Player
	{
		public static ArrayList PlayerList = new ArrayList();
		
		public class PlayerStruct
		{
			public string name;
			public string pid;
			public DateTime LoginTime;
			public TcpClient tcpZone;
			public string ZoneName;
			public string ZoneSW;
			public string[] Chat;
			public string Squad;
		};
		
		public static void AddPlayer(string pname, string pid, string squad)
		{
	    	PlayerStruct p = new PlayerStruct();
	    	p.pid = pid;
	    	p.name = pname;
	    	p.LoginTime = DateTime.Now;
	    	p.ZoneName = Zone.ZoneName;
	    	p.ZoneSW = Zone.Software;
	    	p.tcpZone = (TcpClient)Listen.htRcon[Zone.ZoneName];
	    	p.Squad = squad;
	    	
	    	PlayerList.Add(p);
	    	Zone.PidToName.Add(pid, pname);
		}
		
		/// <summary>
		/// Check to see if this player exists
		/// </summary>
		/// <param name="pname"></param>
		/// <returns>Bool: true or false</returns>
		public static bool Exists(string name)
		{
			// check current player list
			for (int i = 0; i < PlayerList.Count; i++)
	    	{
				PlayerStruct p = (PlayerStruct)PlayerList[i];
				if (p.name == name)
					return true;
			}
			
			// check database
			/* Begin Database Connection */
			DataTable dt = new DataTable();
			
			SQLiteConnection SConnection = new SQLiteConnection();
			SConnection.ConnectionString = SQLite.ConnectionString;
			SConnection.Open();
			
			SQLiteCommand cmd = new SQLiteCommand(SConnection);
			
			cmd.CommandText = @"SELECT * FROM 'players' WHERE name = @pname";
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
				return true;
			else
				return false;
		}
		
		/// <summary>
		/// Retrieve a player's squad based on their username.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetSquad(string name)
		{
			/* Begin Database Connection */
			DataTable dt = new DataTable();
			
			SQLiteConnection SConnection = new SQLiteConnection();
			SConnection.ConnectionString = SQLite.ConnectionString;
			SConnection.Open();
			
			SQLiteCommand cmd = new SQLiteCommand(SConnection);
			
			cmd.CommandText = @"SELECT squad FROM 'players' WHERE name = @pname";
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
				return dt.Rows[0][0].ToString();
			else
				return "";
		}
		
		/// <summary>
		/// Remove the player from both the zone and global player lists.
		/// </summary>
		/// <param name="pid"></param>
		public static void RemovePlayer(string pid)
		{
	    	for (int i = 0; i < PlayerList.Count; i++)
	    	{
	    		PlayerStruct p = (PlayerStruct)PlayerList[i];
	    		if (p.pid == pid && p.ZoneName ==Zone.ZoneName)
	    		{
	    			Zone.PidToName.Remove(pid);
	    			PlayerList.Remove(p);
	    			
	    			if (Zone.PidToName.Count == 0)
	    				Zone.PidToName.Clear();
	    			if (PlayerList.Count == 0)
	    				PlayerList.Clear();
	    			return;
	    		}
	    	}
	    	Zone.PidToName.Remove(pid);
		}
		
		/// <summary>
		/// Return a player's name based on his player id.
		/// </summary>
		/// <param name="pid"></param>
		/// <returns></returns>
		public static string GetPlayerName(string pid)
		{
			return Zone.PidToName[pid].ToString();
		}
		
		public static PlayerStruct GetPlayer(string pid)
		{
	    	for (int i = 0; i < PlayerList.Count; i++)
	    	{
	    		PlayerStruct p = (PlayerStruct)PlayerList[i];
	    		
	    		if (p.pid == pid && p.ZoneName == Zone.ZoneName)
	    			return p;
	    	}
	    	return null;
		}
		
		public static void SetPlayerChat(string pid, string chats)
	    {
	    	for (int i = 0; i < PlayerList.Count; i++)
	    	{
	    		PlayerStruct p = (PlayerStruct)PlayerList[i];
	    		
	    		if (p.pid == pid && p.ZoneName == Zone.ZoneName)
	    		{
	    			chats = chats.Replace(", ", ",");
	    			string[] a_chats = chats.Split(',');
	    			
	    			if (a_chats.Length <= SiusConf.GetSetting.Integer("chatlimit"))
	    				p.Chat = a_chats;
	    			return;
	    		}
	    	}
	    }
		
		public static string[] GetPlayerChat(string pid)
	    {
	    	for (int i = 0; i < PlayerList.Count; i++)
	    	{
	    		PlayerStruct p = (PlayerStruct)PlayerList[i];
	    		
	    		if (p.pid == pid && p.ZoneName == Zone.ZoneName)
	    		{
	    			if (p.Chat != null)
	    				return p.Chat;
	    		}
	    	}
	    	return null;
	    }
		
		public static string GetChannelList(string channel)
	    {
	    	string users = "";
	    	
	    	for (int i = 0; i < PlayerList.Count; i++)
	    	{
	    		PlayerStruct p = (PlayerStruct)PlayerList[i];
	    		
	    		if (SiusUtil.StringInArray(channel, p.Chat))
	    		{
	    			if (string.IsNullOrEmpty(users))
	    				users = p.name;
	    			else
	    				users = users + "," + p.name;
	    		}
	    	}
	    	return users;
	    }
	}
}
