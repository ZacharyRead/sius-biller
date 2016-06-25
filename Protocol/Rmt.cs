/*
 * Sius - Protocol - Remote Private Messages
 */

using System;
using System.Net.Sockets;
using System.IO;

namespace Sius.Protocol
{
	/// <summary>
	/// Forward remote private messages to a player in another zone.
	/// </summary>
	public class Rmt
	{
		public static void Try(string[] parameters)
		{
			//RMT:pid:destination:sound:text
			string sender = Player.GetPlayerName(parameters[1]);
			string message = SiusUtil.Collate(4, parameters);
			
			for (int i = 0; i < Player.PlayerList.Count; i++)
			{
				Player.PlayerStruct p = (Player.PlayerStruct)Player.PlayerList[i];
				if (p.name == parameters[2])
				{
					try
					{
						TcpClient tcpZone = (TcpClient)Listen.htRcon[p.ZoneName];
						StreamWriter swSenderSender;
						swSenderSender = new StreamWriter(tcpZone.GetStream());
						//RMT:pid:sender:sound:text
						swSenderSender.WriteLine("RMT:" + p.pid + ":" + sender + ":" +
						                         parameters[3] + ":" + message);
						swSenderSender.Flush();
						swSenderSender = null;
					}
					catch (Exception e)
					{
						SiusLog.Log(SiusLog.DEBUG, "remote", e.Message);
					}
					SiusLog.Log(SiusLog.INFORMATION, "remote", sender + " sent remote message to " + parameters[2]);
					SiusLog.Log(SiusLog.DEBUG, "remote", "Message: " + message);
					return;
				}
			}
			Message.Send = "MSG:" + parameters[1] + ":0:Unable to locate player.";
			SiusLog.Log(SiusLog.DEBUG, "remote", "Unable to locate and send remote message from " + sender + " to " + parameters[2]);
			SiusLog.Log(SiusLog.DEBUG, "remote", "Message: " + message);
		}
	}
}
