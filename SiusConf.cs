/*
 * Sius - Conf
 * 
 * Note: Add reference path to System.Configuration.
 */

using System;

namespace Sius
{
	/// <summary>
	/// Sius Configuration File Reading and Writing
	/// </summary>
	public class SiusConf
	{
		/// <summary>
		/// Retrieve a specific setting stored in the app.config file.
		/// </summary>
		public class GetSetting
		{
			/// <summary>
			/// Retrieve an boolean value.
			/// </summary>
			/// <param name="setting"></param>
			public static bool Boolean(string setting)
			{
				if (System.Configuration.ConfigurationManager.AppSettings[setting] == "1")
					return true;
				else
					return false;
			}
			
			/// <summary>
			/// Retrieve an integer value.
			/// </summary>
			/// <param name="setting"></param>
			public static int Integer(string setting)
			{
				if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[setting]))
					return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[setting]);
				else return -1;
			}
			
			/// <summary>
			/// Retrieve a string value.
			/// </summary>
			/// <param name="setting"></param>
			public static string String(string setting)
			{
				return System.Configuration.ConfigurationManager.AppSettings[setting];
			}
		}
		
		/// <summary>
		/// Store a specific setting into the app.config file.
		/// </summary>
		public class SetSetting
		{
			/// <summary>
			/// Set an integer value
			/// </summary>
			/// <param name="setting"></param>
			/// <param name="number"></param>
			/// <returns></returns>
			public static void Integer(string setting, int number)
			{
				System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
				config.AppSettings.Settings[setting].Value = Convert.ToString(number);
				config.Save(System.Configuration.ConfigurationSaveMode.Modified);
				System.Configuration.ConfigurationManager.RefreshSection("appSettings");
			}
			
			/// <summary>
			/// Set a string value.
			/// </summary>
			/// <param name="setting"></param>
			/// <param name="text"></param>
			/// <returns></returns>
			public static void String(string setting, string text)
			{
				System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
				config.AppSettings.Settings[setting].Value = text;
				config.Save(System.Configuration.ConfigurationSaveMode.Modified);
				System.Configuration.ConfigurationManager.RefreshSection("appSettings");
			}
		}
		
		/// <summary>
		/// Reload the configuration file (e.g. if a change was made externally)
		/// </summary>
		public static void ReloadConf()
		{
			System.Configuration.ConfigurationManager.RefreshSection("appSettings");
			return;
		}
	}
}
