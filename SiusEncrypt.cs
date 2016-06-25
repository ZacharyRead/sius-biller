/*
 * Sius - Encryption
 */

using System;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Sius
{
	/// <summary>
	/// Sius Encryption Methods
	/// </summary>
	public class SiusEncrypt
	{
		/// <summary>
		/// Encrypt the password according to the hash method chosen, and optionally
		/// salt the password as to nullify the effect of precomputational attacks such
		/// as Rainbow tables.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string Encrypt(string text, string name, string table)
		{
			if (SiusConf.GetSetting.Boolean("salt"))
			{
				/* Begin Database Connection */
				DataTable dt = new DataTable();
				
				SQLiteConnection SConnection = new SQLiteConnection();
				SConnection.ConnectionString = SQLite.ConnectionString;
				SConnection.Open();
				
				SQLiteCommand cmd = new SQLiteCommand(SConnection);
				
				cmd.CommandText = @"SELECT salt FROM " + table + " WHERE name = @vname";
				SQLiteParameter vname = new SQLiteParameter("@vname");
				cmd.Parameters.Add(vname);
				vname.Value = name;
				
				SQLiteDataReader Reader = cmd.ExecuteReader();
				dt.Load(Reader);
				Reader.Close();
				
				SConnection.Close();
				/* End Database Connection */
				
				string salt = "";
				
				if (dt.Rows.Count > 0)
					salt = dt.Rows[0][0].ToString();
				
				if (string.IsNullOrEmpty(salt))
				{
					salt = RandomSalt();					
					/* Begin Database Connection */					
					SConnection.Open();
					
					SQLiteCommand cmdsalt = new SQLiteCommand(SConnection);
					
					cmdsalt.CommandText = @"UPDATE " + table + " SET salt = @asalt WHERE name = @vname";
					SQLiteParameter asalt = new SQLiteParameter("@asalt");
					cmdsalt.Parameters.Add(asalt);
					cmdsalt.Parameters.Add(vname);
					asalt.Value = salt;
					vname.Value = name;
					
					cmdsalt.ExecuteNonQuery();
					
					SConnection.Close();
					SConnection.Dispose();
					/* End Database Connection */
				}
				text += salt;
			}
			
			string encrypted;
			string type = SiusConf.GetSetting.String("hash");
			
			switch (type)
			{
				case "md5" :
					encrypted = md5encrypt(text);
					break;
				case "sha1" :
					encrypted = sha1encrypt(text);
					break;
				case "sha256" :
					encrypted = sha256encrypt(text);
					break;
				case "sha384" :
					encrypted = sha384encrypt(text);
					break;
				case "sha512" :
					encrypted = sha512encrypt(text);
					break;
				case "ripemd160" :
					encrypted = ripemdencrypt(text);
					break;
				default :
					encrypted = sha512encrypt(text);
					break;
			}
			
			return encrypted;
		}
		
		private static string md5encrypt(string phrase)
		{
			UTF8Encoding encoder = new UTF8Encoding();
			MD5CryptoServiceProvider md5hasher = new MD5CryptoServiceProvider();
			byte[] hashedDataBytes = md5hasher.ComputeHash(encoder.GetBytes(phrase));
			return byteArrayToString(hashedDataBytes);
		}
		
		private static string sha1encrypt(string phrase)
		{
			UTF8Encoding encoder = new UTF8Encoding();
			SHA1CryptoServiceProvider sha1hasher = new SHA1CryptoServiceProvider();
			byte[] hashedDataBytes = sha1hasher.ComputeHash(encoder.GetBytes(phrase));
			return byteArrayToString(hashedDataBytes);
		}
		private static string sha256encrypt(string phrase)
		{
			UTF8Encoding encoder = new UTF8Encoding();
			SHA256Managed sha256hasher = new SHA256Managed();
			byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
			return byteArrayToString(hashedDataBytes);
		}
		
		private static string sha384encrypt(string phrase)
		{
			UTF8Encoding encoder = new UTF8Encoding();
			SHA384Managed sha384hasher = new SHA384Managed();
			byte[] hashedDataBytes = sha384hasher.ComputeHash(encoder.GetBytes(phrase));
			return byteArrayToString(hashedDataBytes);
		}
		
		private static string sha512encrypt(string phrase)
		{
			UTF8Encoding encoder = new UTF8Encoding();
			SHA512Managed sha512hasher = new SHA512Managed();
			byte[] hashedDataBytes = sha512hasher.ComputeHash(encoder.GetBytes(phrase));
			return byteArrayToString(hashedDataBytes);
		}
		
		private static string ripemdencrypt(string phrase)
		{
			UTF8Encoding encoder = new UTF8Encoding();
			RIPEMD160Managed ripemdhasher = new RIPEMD160Managed();
			byte[] hashedDataBytes = ripemdhasher.ComputeHash(encoder.GetBytes(phrase));
			return byteArrayToString(hashedDataBytes);
		}
		
		private static string byteArrayToString(byte[] inputArray)
		{
			StringBuilder output = new StringBuilder("");
			for (int i = 0; i < inputArray.Length; i++)
			{
				output.Append(inputArray[i].ToString("X2"));
			}
			return output.ToString();
		}
		
		private static string RandomSalt()
		{
			byte[] salt = new byte[10];
			
			Random random = new Random();
			
			for (int i = 0; i < salt.Length ; i += 2)
			{
				int character = random.Next(0xD7FF);
				salt[i+1] = (byte)((character & 0xFF00) >> 8);
				salt[i] = (byte)(character & 0xFF);
			}
			return Encoding.Unicode.GetString(salt);
		}
	}
}
