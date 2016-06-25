/*
 * Sius - Log
 */

using System;

namespace Sius.Protocol
{
	/// <summary>
	/// Handle logs sent from the zone.
	/// </summary>
	public class Log
	{
		/// <summary>
		/// Handle logs sent from the zone.
		/// </summary>
		public static void Try(string[] parameters)
		{
			//TODO: Send this log to all online operators
			SiusLog.Log(SiusLog.WARNING, "zone", "(" + Zone.ZoneName + ") " +
			            SiusUtil.Collate(1, parameters));
		}
	}
}
