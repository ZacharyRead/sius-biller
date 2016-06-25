/*
 * Sius - Zone
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;

namespace Sius
{
	/// <summary>
	/// Description of Zone.
	/// </summary>
	public class Zone
	{
		// the name of the current zone
		[ThreadStatic]
		public static string ZoneName;
		
		// the zone software name and version (for backwards compatibility purposes)
		[ThreadStatic]
		public static string Software;
		
		// a quick lookup table of player ids and names
		[ThreadStatic]
		public static Hashtable PidToName =  new Hashtable();
		
		/// <summary>
		/// Add a zone to the hash table.
		/// </summary>
		/// <param name="tcpZone"></param>
		/// <param name="ZoneName"></param>
		public static void AddZone(TcpClient tcpZone)
		{
			// First add the username and associated connection to both hash tables
			PidToName = new Hashtable();
			Listen.htZones.Add(tcpZone, ZoneName);
			Listen.htRcon.Add(ZoneName, tcpZone);
		}
		
		/// <summary>
		/// Remove the zone from the hash table, and all players associated with it.
		/// </summary>
		/// <param name="tcpZone"></param>
		public static void RemoveZone(TcpClient tcpZone)
		{
			if (Listen.htZones[tcpZone] != null)
			{
				Listen.htRcon.Remove(Listen.htZones[tcpZone]);
				Listen.htZones.Remove(tcpZone);
				
				//remove players
				for (int i = 0; i < Player.PlayerList.Count; i++)
				{
					Player.PlayerStruct p = (Player.PlayerStruct)Player.PlayerList[i];
					
					if (p.tcpZone == tcpZone)
					{
						Player.PlayerList.Remove(p);
					}
				}
			}
		}
		
		/// <summary>
		/// Broadcast a direct tcp message to all connected zones.
		/// </summary>
		/// <param name="Message"></param>
	    public static void BroadcastMessage(string Message)
    	{
        	StreamWriter swSenderSender;
			
			// Create an array of TCP clients, the size of the number of zones we have
			TcpClient[] tcpClients = new TcpClient[Listen.htRcon.Count];
			// Copy the TcpClient objects into the array
			Listen.htRcon.Values.CopyTo(tcpClients, 0);
			// Loop through the list of TCP clients
			for (int i = 0; i < tcpClients.Length; i++)
			{
				try
				{
					// If the message is blank or the connection is null, break out
					if (Message.Trim() == "" || tcpClients[i] == null)
					{
						continue;
					}
					// Send the message to the current user in the loop
					swSenderSender = new StreamWriter(tcpClients[i].GetStream());
					swSenderSender.WriteLine(Message);
					swSenderSender.Flush();
					swSenderSender = null;
				}
				catch // If there was a problem, the zone isn't there, remove them
				{
					RemoveZone(tcpClients[i]);
				}
			}
		}
	}
}
