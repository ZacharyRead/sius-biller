/*
 * Sius - Listen
 */

using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;

namespace Sius
{
	/// <summary>
	/// Listens for incoming TCP connections and messages and
	/// responds according to the protocol.
	/// </summary>
	public class Listen
	{
		// The hash table containing the Zone connections
		public static Hashtable htZones = new Hashtable();
		public static Hashtable htRcon = new Hashtable();
		
		TcpClient tcpClient;
		
		// The thread that will hold the connection listener
		private Thread thrListener;
		
		// Listening for connections
		private TcpListener Listening;
		
		/// <summary>
		/// Begin listening for incoming TCP connections on the specified port number
		/// </summary>
		public void StartListening()
		{
			int port = SiusConf.GetSetting.Integer("port");
			
			// TCP port numbers are 16 bits in length, so they can only be within the range of 0 - 65535
			if (port <= 0 || port >= 65535)
			{
				SiusLog.Log(SiusLog.ERROR,"server","An error occured when trying to define a port number. Please make sure the port is within the range of 1 - 65534");
				Sius.Connected = false;
				return;
			}
			
			string address = SiusConf.GetSetting.String("ip");
			IPAddress localAddr;
			
			// check if the IP Address is valid
			if (SiusUtil.IsValidIP(address) == false)
			{
				SiusLog.Log(SiusLog.ERROR, "server", "Unable to retrieve IP Address. Please make sure that you are using a valid IP Address such as 127.0.0.1");
				Sius.Connected = false;
				return;
			}
			else
				localAddr = IPAddress.Parse(address);
			
			// Log our port and ip address
			SiusLog.Log(SiusLog.INFORMATION, "server", "Server port: " + port + ", Server IP: " + address);
			
			// Create TCP Listener and start checking for incoming connections
			try
			{
				Listening = new TcpListener(localAddr, port);
				Listening.Start();
			}
			catch (SocketException e)
			{
				SiusLog.Log(SiusLog.ERROR, "connect", e.Message);
				Sius.Connected = false;
				return;
			}
			
			// Server is connected!
			Sius.Connected = true;
			
			// Start a new thread
			thrListener = new Thread(KeepListening);
			thrListener.Start();
		}
		
		private void KeepListening()
		{
			// While the server is running
			while (Sius.Connected == true)
			{
				try
				{
					// Accept a pending connection
					tcpClient = Listening.AcceptTcpClient();
					
					// Create a new instance of Connection
					Connection newConnection = new Connection(tcpClient);
				}
				catch (SocketException e)
				{
					SiusLog.Log(SiusLog.WARNING, "connect", e.Message);
				}
			}
		}
		
		/// <summary>
		/// RequestStop() will set the connected status to false,
		/// abort the thread, and close the socket.
		/// </summary>
		public void RequestStop()
		{
			if (thrListener != null && thrListener.IsAlive) // thread is active
			{
				// set event "Stop"
				Sius.Connected = false;
				thrListener.Abort();
				Listening.Stop();
				
				TcpClient[] tcpClients = new TcpClient[htRcon.Count];
				htRcon.Values.CopyTo(tcpClients, 0);
				
				for (int i = 0; i < tcpClients.Length; i++)
				{
					try
					{
						if (tcpClients[i] != null)
							tcpClients[i].Close();
					}
					catch (Exception e)
					{
						SiusLog.Log(SiusLog.DEBUG, "disconnect", e.Message);
					}
				}
			}
		}
	}
	
	class Connection
	{
		TcpClient tcpClient;
		// The thread that will send information to the client
		private Thread thrSender;
		private StreamReader srReceiver;
		private StreamWriter swSender;
		private string strReceive;

		// The constructor of the class takes in a TCP connection
		public Connection(TcpClient tcpCon)
		{
			tcpClient = tcpCon;
			thrSender = new Thread(AcceptClient);
			thrSender.IsBackground = true; //make this a background thread for clean termination
			thrSender.Start();
		}
		
		private void CloseConnection()
		{
			// Close the currently open objects
			tcpClient.Close();
			srReceiver.Close();
			swSender.Close();
			srReceiver.Dispose();
			swSender.Dispose();
			thrSender.Abort();
		}
		
		// Occurs when a new client is accepted
		private void AcceptClient()
		{
			srReceiver = new StreamReader(tcpClient.GetStream());
			swSender = new StreamWriter(tcpClient.GetStream());
			
			// Read the account information from the client
			strReceive = srReceiver.ReadLine();
			
			// We received a response from the zone
			if (!string.IsNullOrEmpty(strReceive))
			{
				//SiusLog.Log(SiusLog.INFORMATION, "zone", strReceive);
				string respond = Protocol.Connect.Try(strReceive);
				
				if (!string.IsNullOrEmpty(respond))
				{
					//SiusLog.Log(SiusLog.DEBUG, "zone", respond); //CONNECTOK
					swSender.WriteLine(respond);
					swSender.Flush();
				}
				
				if (respond.StartsWith("CONNECTBAD"))
					CloseConnection();
			}
			else
			{
				CloseConnection();
				return;
			}
			
			// Add the zone to the hash table
	 		Zone.AddZone(tcpClient);
			
//	 		try
//			{
				// Keep waiting for a message from the user
				while ((Sius.Connected == true) && thrSender.IsAlive) 
				{
					if (!string.IsNullOrEmpty(strReceive = srReceiver.ReadLine()))
					{
						//SiusLog.Log(SiusLog.INFORMATION, "zone", strReceive);
						string respond = Message.Receive(strReceive);
						
						if (!string.IsNullOrEmpty(respond))
						{
							try
							{
								//SiusLog.Log(SiusLog.DEBUG, "zone", respond);
								swSender.WriteLine(respond);
								swSender.Flush();
							}
							catch (Exception e)
							{
								SiusLog.Log(SiusLog.DEBUG, "connect", e.Message);
							}
						}
					}
					else
						break;
				}
//			}
//	 		catch (Exception e)
//			{
//	 			SiusLog.Log(SiusLog.MALICIOUS, "disconnect", e.Message);
//			}
	 		//Disconnect
	 		Zone.RemoveZone(tcpClient);
	 		SiusLog.Log(SiusLog.INFORMATION, "disconnect", Zone.ZoneName + " disconnected from network.");
	 		CloseConnection();
		}
	}
}
