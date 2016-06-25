/*
 * Sius - Protocol - Commands - Messages
 */

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Display a player's ?messages, or send a player a new message.
	/// </summary>
	public class Messages
	{
		public static void Try(string[] parameters)
		{
			//CMD:pid:cmdname:name:text
			if (string.IsNullOrEmpty(parameters[3]))
			{
				Player.PlayerStruct p = Player.GetPlayer(parameters[1]);
				
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"UPDATE 'messages' SET flag = 1 WHERE name = @pname;" +
					"SELECT * FROM 'messages' WHERE name = @pname";
				SQLiteParameter pname = new SQLiteParameter("@pname");
				cmd.Parameters.Add(pname);
				pname.Value = p.name;
				
				SQLiteDataReader Reader = cmd.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				
				SConnection.Close();
				/* End Database Connection */
				
	   		 	// The player has no new messages
	   		 	if (dt.Rows.Count == 0)
	   		 	{
	   		 		Message.Send = "MSG:" + parameters[1] + ":0:You have no new messages.";
	   		 		SiusLog.Log(SiusLog.DEBUG, "?messages", "[" + p.name + "] has no new messages.");
	   		 		return;
	   		 	}
	   		 	
	   		 	TcpClient tcpZone = (TcpClient)p.tcpZone;
	   		 	StreamWriter swSenderSender;
	   		 		
	   		 	swSenderSender = new StreamWriter(tcpZone.GetStream());
	   		 	
	   		 	for (int a = 0; a < dt.Rows.Count; a++)
	   		 	{
					string time = dt.Rows[a][3].ToString();
					string sender = dt.Rows[a][1].ToString();
					string message = dt.Rows[a][2].ToString();
	   		 		
	   		 		swSenderSender.WriteLine("MSG:" + parameters[1] + ":0:" +
	   		 								"[" + time + "] " + sender + ": " +
	   		 							   message);
	   		 	}
	   		 	swSenderSender.Flush();
	   		 	swSenderSender = null;
	   		 	
	   		 	SiusLog.Log(SiusLog.INFORMATION, "?messages", "Sent [" + p.name + "] their messages.");
	   		 	return;
			}
			else
			{
				if (parameters.Length < 5)
					return;
				
				string receiver = parameters[3];
				string message = SiusUtil.Collate(4, parameters);
				string name = Player.GetPlayerName(parameters[1]);
				
				/*SQLite.SendQuery("INSERT INTO 'messages' (name,sender,message,time) " +
								 "VALUES ('" + receiver + "','" +
								 name + "','" + message + "','" +
								 System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "');");
				
				SiusLog.Log(SiusLog.DEBUG, "messages", "[" + name + "] sent [" + receiver +
							"] the following message: " + message);*/
				
				
				
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				SQLiteParameter apname = new SQLiteParameter("@pname");
				cmd.Parameters.Add(apname);
				apname.Value = name;
				
				//Check maximum number of messages sent
				if (!string.IsNullOrEmpty(SiusConf.GetSetting.String("maxsent")))
				{
					SConnection.Open();
					cmd.CommandText = @"SELECT * FROM messages WHERE name = @pname";
					
					SQLiteDataReader Reader = cmd.ExecuteReader();
					dt.Load(Reader);
					Reader.Close();
					
					SConnection.Close();
					
					int rows = 0;
					
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						if (dt.Rows[i][4].ToString() == "0")
							rows++;
					}
					
					if (rows >= SiusConf.GetSetting.Integer("maxsent"))
					{
						Message.Send = "MSG:" + parameters[1] + ":0:Error: Too many messages have been sent out."
							+ " Please wait a while before trying again";
						
						SConnection.Dispose();
						
						SiusLog.Log(SiusLog.INFORMATION, "?message", "Unable to send <" + receiver + "> " + name
						            + "'s message (too many pending messages sent).");
						
						return;
					}
					dt.Clear();
				}
				
				
				//Check maximum number of messages received
				if (!string.IsNullOrEmpty(SiusConf.GetSetting.String("maxinbox")))
				{
					SConnection.Open();
					cmd.CommandText = @"SELECT * FROM messages WHERE sender = @pname";
					
					SQLiteDataReader Reader = cmd.ExecuteReader();
					dt.Load(Reader);
					Reader.Close();
					
					SConnection.Close();
					int rows = 0;
					
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						if (dt.Rows[i][4].ToString() == "0")
							rows++;
					}
					
					if (rows >= SiusConf.GetSetting.Integer("maxinbox"))
					{
						Message.Send = "MSG:" + parameters[1] + ":0:Error: This player's message inbox is full."
							+ " Please try again at another time.";
						
						SConnection.Dispose();
						
						SiusLog.Log(SiusLog.DEBUG, "?message", "Unable to send <" + receiver + "> " + name + "'s message (inbox full).");
						
						return;
					}
					dt.Clear();
				}
				
				//Send the message
				SConnection.Open();
				cmd.CommandText = @"INSERT INTO 'messages' (name,sender,message,time) " +
								 "VALUES (@receiver,@pname,@message,@utc);";
				SQLiteParameter areceiver = new SQLiteParameter("@receiver");
				SQLiteParameter amessage = new SQLiteParameter("@message");
				SQLiteParameter autc = new SQLiteParameter("@utc");
				cmd.Parameters.Add(areceiver);
				cmd.Parameters.Add(amessage);
				cmd.Parameters.Add(autc);
				areceiver.Value = receiver;
				amessage.Value = message;
				autc.Value = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
				
				cmd.ExecuteNonQuery();
				
				SConnection.Close();
				SConnection.Dispose();
				/* End Database Connection */
				
				
				Message.Send = "MSG:" + parameters[1] + ":0:Message has been sent to " + receiver;
				SiusLog.Log(SiusLog.INFORMATION, "?message", name + " sent " + receiver + " a message.");
				SiusLog.Log(SiusLog.DEBUG, "?message", "Message: " + message);
			}
		}
	}
}
