/*
 * Sius - Util
 * 
 * This file is designed to provide accessibility to functions that
 * may or may not be used in several different places. In other words,
 * it serves as a convenient way to avoid repetition.
 */

using System;

namespace Sius
{
	/// <summary>
	/// Sius Utility
	/// </summary>
	public class SiusUtil
	{
		/// <summary>
		/// Count the number of occurences of one string within another one.
		/// Returns an int specifying the number of separate matches found.
		/// </summary>
		public static int CountStringOccurrences(string text, string pattern)
		{
			// Loop through all instances of the string 'text'.
			int count = 0;
			int i = 0;
			while ((i = text.IndexOf(pattern, i)) != -1)
			{
				i += pattern.Length;
				count++;
			}
			return count;
		}
		
		/// <summary>
		/// Check whether a string consists of a valid IP Address or not.
		/// </summary>
		/// <param name="addr"></param>
		/// <returns></returns>
		public static bool IsValidIP(string address)
		{
		    System.Net.IPAddress ip;
		    bool valid = false;
		    
		    //check to make sure an ip address was provided
		    if (!string.IsNullOrEmpty(address))
		    {
		        valid = System.Net.IPAddress.TryParse(address, out ip);
		    }
		    return valid;
		}
		
		/// <summary>
		/// Checks if a string within an array exists.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="array"></param>
		/// <returns></returns>
		public static bool StringInArray(string text, string[] array)
		{			
			if (((System.Collections.Generic.IList<string>)array).Contains(text))
				return true;
			else
				return false;
		}
		
		/// <summary>
		/// Takes all the strings in the array starting from a certain point,
		/// and joins them together.
		/// </summary>
		/// <param name="last"></param>
		/// <param name="array"></param>
		/// <returns></returns>
		public static string Collate(int last, string[] array)
		{
			string message = array[last];
			for (int i = last + 1; i < array.Length; i++)
			{
				message += ":" + array[i];
			}
			return message;
		}
	}
}
