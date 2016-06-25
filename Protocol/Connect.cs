/*
 * Sius - Protocol - Connect
 */

using System;

namespace Sius.Protocol
{
	/// <summary>
	/// Process the information when a zone attempts to connect.
	/// </summary>
	public class Connect
	{
		public static string Try(string message)
		{
			// CONNECT:version:swname:zonename:hostname:password
			string[] parameters = message.Split(':');
			
			string protocol = SiusConf.GetSetting.String("protocol");
			string server = SiusConf.GetSetting.String("server");
			string[] _protocol = protocol.Split(',');
			string[] _server = server.Split(',');
			
			Zone.ZoneName = parameters[3];
			Zone.Software = parameters[2];
			
			// Check if the number of parameters expected is different (e.g. zone name containing ':')
			if (parameters.Length != 6)
			{
				message = "CONNECTBAD:Sius:" + SiusConf.GetSetting.String("network") + ":Invalid connection protocol.";
				SiusLog.Log(SiusLog.WARNING, "connect", Zone.ZoneName + " failed to connect: Invalid connection protocol.");
				return message;
			}
			
			// Check if the server is using the appropriate protocol
			if ((!SiusUtil.StringInArray(parameters[1], _protocol)) && (!string.IsNullOrEmpty(protocol)))
			{
				message = "CONNECTBAD:Sius:" + SiusConf.GetSetting.String("network") + ":Invalid protocol version.";
				SiusLog.Log(SiusLog.WARNING, "connect", Zone.ZoneName + " failed to connect: Unrecognized protocol version.");
				return message;
			}
			
			// Check if the server is using a recognized type
			if ((!SiusUtil.StringInArray(parameters[2], _server)) && (!string.IsNullOrEmpty(server)))
			{
				message = "CONNECTBAD:Sius:" + SiusConf.GetSetting.String("network") + ":Invalid server version.";
				SiusLog.Log(SiusLog.WARNING, "connect", Zone.ZoneName + " failed to connect: Invalid server version.");
				return message;
			}
			
			// Check if there are too many zones connected.
			if (!string.IsNullOrEmpty(SiusConf.GetSetting.String("maxzones")))
			{
				if (Listen.htZones.Count >= SiusConf.GetSetting.Integer("maxzones"))
				{
					message = "CONNECTBAD:Sius:" + SiusConf.GetSetting.String("network") + ":There are too many zones currently connected.";
					SiusLog.Log(SiusLog.WARNING, "connect", Zone.ZoneName + " failed to connect: There are too many zones currently connected.");
					return message;
				}
			}
			
			// Verify the password
			if (!string.IsNullOrEmpty(SiusConf.GetSetting.String("password")))
			{
				if (parameters[5] != SiusConf.GetSetting.String("password"))
				{
					message = "CONNECTBAD:Sius:" + SiusConf.GetSetting.String("network") + ":Authentication failed: Invalid Password.";
					SiusLog.Log(SiusLog.WARNING, "connect", Zone.ZoneName + " failed to connect: Invalid Password.");
					return message;
				}
			}
			
			// Check if the zone contains the illegal character ',' or if it's empty
			if (Zone.ZoneName.Contains(",") ||  string.IsNullOrEmpty(Zone.ZoneName))
			{
				message = "CONNECTBAD:Sius:" + SiusConf.GetSetting.String("network") + ":The selected zone name is not valid.";
				SiusLog.Log(SiusLog.WARNING, "connect", Zone.ZoneName + " failed to connect: The selected zone name is not valid.");
				return message;
			}
			
			// Check if a zone with this name already exists
			if (Listen.htRcon.Contains(Zone.ZoneName))
			{
				message = "CONNECTBAD:Sius:" + SiusConf.GetSetting.String("network") + ":A zone with this name already exists.";
				SiusLog.Log(SiusLog.WARNING, "connect", Zone.ZoneName + " failed to connect: a zone with this name already exists.");
				return message;
			}
			
			message = "CONNECTOK:Sius:" + SiusConf.GetSetting.String("network");
			SiusLog.Log(SiusLog.INFORMATION, "connect", Zone.ZoneName + " successfully connected to the network.");
			return message;
		}
	}
}
