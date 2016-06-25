/*
 * Sius - Protocol - Chat
 */

using System;
using System.IO;
using System.Net.Sockets;

namespace Sius.Protocol
{
	/// <summary>
	/// Handle player chat messages.
	/// </summary>
	public class Chat
	{
		public static void Try(string[] parameters)
		{
			//CHAT:pid:channel:sound:text
			
			string sender = ""; // the name of the sender
			string channel = ""; // the name of the chat channel
			
			Player.PlayerStruct p = Player.GetPlayer(parameters[1]);
			
			// Check if the player is sending to a valid channel
			if (p.Chat != null)
	    	{
	    		sender = p.name;
	    		int num;
	    		try
	    		{
	    			num = Convert.ToInt32(parameters[2]);
	    		}
	    		catch
	    		{
	    			num = 0;
	    		}
	    		
	    		if (num < p.Chat.Length)
	    			channel = p.Chat[num];
	    		else
	    			return;
	    	}
			
			if (string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(sender))
				return;
			
			// Send the message to every player on that chat
			for (int i = 0; i < Player.PlayerList.Count; i++)
			{
				Player.PlayerStruct pp = (Player.PlayerStruct)Player.PlayerList[i];
				
				if (SiusUtil.StringInArray(channel, pp.Chat) && pp.name != sender)
				{
					try
					{
						TcpClient tcpZone = (TcpClient)Listen.htRcon[pp.ZoneName];
						
						StreamWriter swSenderSender;
						swSenderSender = new StreamWriter(tcpZone.GetStream());
						//CHATTXT:channel:sender:sound:text
						//CHAT:pid:number
						swSenderSender.WriteLine("CHATTXT:" + channel + ":" + sender + ":" +
						                         parameters[3] + ":" + SiusUtil.Collate(4, parameters));
						if (pp.ZoneSW == "asss 1.4.3" || pp.ZoneSW == "asss 1.4.2") // Older versions are broken.
							swSenderSender.WriteLine("CHAT:" + pp.pid + ":" + Array.IndexOf(pp.Chat, channel).ToString() + ":");
						else
							swSenderSender.WriteLine("CHAT:" + pp.pid + ":" + Array.IndexOf(pp.Chat, channel).ToString());
						swSenderSender.Flush();
						swSenderSender = null;
					}
					catch (Exception e)
					{
						SiusLog.Log(SiusLog.DEBUG, "chat", e.Message);
					}
				}
			}
			SiusLog.Log(SiusLog.INFORMATION, "chat", sender + " to " + channel + ": " + SiusUtil.Collate(4, parameters));
			return;
		}
	}
}
