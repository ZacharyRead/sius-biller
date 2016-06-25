/*
 * Sius - Protocol - Commands - Player Chats
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Sius.Protocol.Commands
{
	/// <summary>
	/// Send a user his chat list, or set his chats.
	/// </summary>
	public class Chat
	{
		public static void Try(string[] parameters)
		{
			if (string.IsNullOrEmpty(parameters[3]))
			{
				string[] chats = Player.GetPlayerChat(parameters[1]);
				if (chats != null)
				{
					SendChatList(chats, parameters);
					SiusLog.Log(SiusLog.INFORMATION, "?chat", "Retrieved " + Player.GetPlayerName(parameters[1]) + "'s chat list.");
					return;
				}
				else
					Message.Send = "MSG:" + parameters[1] + ":0:Empty.";
			}
			else
			{
				Player.SetPlayerChat(parameters[1], SiusUtil.Collate(3, parameters));
				SiusLog.Log(SiusLog.INFORMATION, "?chat", "Set " + Player.GetPlayerName(parameters[1]) + "'s chat list.");
			}
		}
		
		/// <summary>
		/// Send the player his chatlist.
		/// </summary>
		/// <param name="chats"></param>
		private static void SendChatList(string[] chats, string[] parameters)
		{
			TcpClient tcpZone = (TcpClient)Listen.htRcon[Zone.ZoneName];
	    	try
	    	{
	    		StreamWriter sw = new StreamWriter(tcpZone.GetStream());
	    		
	    		for (int i = 0; i < chats.Length; i++)
	    		{
	    			sw.WriteLine("MSG:" + parameters[1] + ":0:" +
	    			                         chats[i] + ": " + Player.GetChannelList(chats[i]));
	    		}
	    		
		    	sw.Flush();
		    	sw = null;
	    	}
	    	catch (Exception e)
	    	{
	    		SiusLog.Log(SiusLog.DEBUG, "chatlist", e.Message);
	    	}
		}
	}
}
