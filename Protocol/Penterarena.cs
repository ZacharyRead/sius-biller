/*
 * Sius - Protocol - Player Enter Arena
 */

using System;
using System.Net.Sockets;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol
{
	/// <summary>
	/// When a player enters the arena, send him his banner and information.
	/// </summary>
	public class Penterarena
	{
		public static void Try(string[] parameters)
		{
			//PENTERARENA:pid
			
			string pname = Player.GetPlayerName(parameters[1]);
			if (!String.IsNullOrEmpty(pname))
			{
				/*string banner = SQLite.GetQuery("SELECT banner FROM 'banners' WHERE name = '" +
				                                pname + "';");*/
				
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"SELECT banner FROM 'banners' WHERE name = @apname";
				SQLiteParameter apname = new SQLiteParameter("@apname");
				cmd.Parameters.Add(apname);
				apname.Value = pname;
				
				SQLiteDataReader Reader = cmd.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				
				SConnection.Close();
				SConnection.Dispose();
				/* End Database Connection */
				
				if (dt.Rows.Count > 0)
				{
					SendGreeting(parameters);
					CheckMessages(parameters);
					Message.Send = "BNR:" + parameters[1] + ":" + dt.Rows[0][0].ToString();
					SiusLog.Log(SiusLog.INFORMATION, "arena", "(" + Zone.ZoneName + ") " + pname + " entered arena. Sent greeting, banner, and checked messages.");
					
					return;
				}
				SiusLog.Log(SiusLog.DEBUG, "arena", "Unable to retrieve " + pname + "'s banner information from database.");
			}
			
			SendGreeting(parameters);
			CheckMessages(parameters);
			SiusLog.Log(SiusLog.INFORMATION, "arena", "(" + Zone.ZoneName + ") " + pname + " entered arena. Sent greeting and checked messages.");
			return;
		}
		
		/// <summary>
		/// Send the biller greeting message to the player.
		/// </summary>
		private static void SendGreeting(string[] parameters)
		{
			string greeting = SiusConf.GetSetting.String("greeting");
			if (String.IsNullOrEmpty(greeting))
				return;
			
			//MSG:pid:sound:text
			TcpClient tcpZone = (TcpClient)Listen.htRcon[Zone.ZoneName];
	    	try
	    	{
	    		StreamWriter swSenderSender;
	    		swSenderSender = new StreamWriter(tcpZone.GetStream());
	    		swSenderSender.WriteLine("MSG:" + parameters[1] + ":" +
	    		                         SiusConf.GetSetting.String("greetsound") + ":" + greeting);
		    	swSenderSender.Flush();
		    	swSenderSender = null;
	    	}
	    	catch (Exception e)
	    	{
	    		SiusLog.Log(SiusLog.DEBUG, "greet", e.Message);
	    	}
	    	return;
		}
		
		/// <summary>
		/// When a player logs in, check if he has any new messages unread.
		/// </summary>
		private static void CheckMessages(string[] parameters)
		{
			/*System.Data.DataTable result = SQLite.GetMultiQuery("SELECT * FROM 'messages'" +
			                                                    "WHERE name = '" + name + "';");*/
			
			/* Begin Database Connection */
			DataTable dt = new DataTable();
			
			SQLiteConnection SConnection = new SQLiteConnection();
			SConnection.ConnectionString = SQLite.ConnectionString;
			SConnection.Open();
			
			SQLiteCommand cmd = new SQLiteCommand(SConnection);
			
			cmd.CommandText = @"SELECT * FROM 'messages' WHERE name = @pname";
			SQLiteParameter pname = new SQLiteParameter("@pname");
			cmd.Parameters.Add(pname);
			pname.Value = Player.GetPlayerName(parameters[1]);
			
			SQLiteDataReader Reader = cmd.ExecuteReader();
			dt.Load(Reader);
			Reader.Close();
			
			SConnection.Close();
			SConnection.Dispose();
			/* End Database Connection */
			
			if (dt.Rows.Count != 0)
			{
				TcpClient tcpZone = (TcpClient)Listen.htRcon[Zone.ZoneName];
	    		try
	    		{
	    			StreamWriter swSenderSender;
	    			swSenderSender = new StreamWriter(tcpZone.GetStream());
	    			swSenderSender.WriteLine("MSG:" + parameters[1] + ":0:You have " +
	    			                         dt.Rows.Count.ToString() + " new messages. " +
	    			                        "Type ?messages to read them.");
		    		swSenderSender.Flush();
		    		swSenderSender = null;
	    		}
	    		catch (Exception e)
	    		{
	    			SiusLog.Log(SiusLog.DEBUG, "CheckMessages", e.Message);
	    		}
			}
		}
	}
}
