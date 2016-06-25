/*
 * Sius - Protocol - Commands - Squad List
 */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// List all members within the squad.
	/// </summary>
	public class Squadlist
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
				
				cmd.CommandText = @"SELECT name FROM players WHERE squad = @nsquad";
				SQLiteParameter nsquad = new SQLiteParameter("@nsquad");
				cmd.Parameters.Add(nsquad);
				nsquad.Value = p.Squad;
				
				SQLiteDataReader Reader = cmd.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				
				SConnection.Close();
				/* End Database Connection */
				
				if (dt.Rows.Count > 0)
				{
					string list = "";
					for(int i = 0; i < dt.Rows.Count; i++)
					{
						list += dt.Rows[i][0].ToString();
						if ((i + 1) < dt.Rows.Count)
							list += ", ";
					}
					
					string message = "MSG:" + parameters[1] + ":0:Members: " + list;
					
					if (message.Length >= 250)
					{
						SendSquadList(list.Split(", ".ToCharArray()), parameters);
					}
					else
						Message.Send = message;
					
					SiusLog.Log(SiusLog.INFORMATION, "?squadlist", p.name + " requested <" + p.Squad + ">'s member list.");
				}
				else
					SiusLog.Log(SiusLog.WARNING, "?squadlist", "An error occured while trying to locate " + p.name + "'s squad member list.");
			}
		}
		
		
		/// <summary>
		/// Send the player his squad list.
		/// </summary>
		/// <param name="chats"></param>
		private static void SendSquadList(string[] list, string[] parameters)
		{
			TcpClient tcpZone = (TcpClient)Listen.htRcon[Zone.ZoneName];
	    	try
	    	{
	    		StreamWriter sw = new StreamWriter(tcpZone.GetStream());
	    		
	    		string message = "";
	    		
	    		for (int i = 0; i < list.Length; i++)
	    		{
	    			if (message.Length < 230)
	    				message += list[i] + ", ";
	    			else
	    			{
	    				sw.WriteLine("MSG:" + parameters[1] + ":0:Members: " + message);
	    				message = "";
	    			}
	    		}
	    		
		    	sw.Flush();
		    	sw = null;
	    	}
	    	catch (Exception e)
	    	{
	    		SiusLog.Log(SiusLog.DEBUG, "squadlist", e.Message);
	    	}
		}
	}
}
