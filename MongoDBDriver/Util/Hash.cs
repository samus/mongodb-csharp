/*
 * User: Sedward
 */

using System;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver.Util
{
	public class Hash
	{
		public static string MD5Hash(string text){
			MD5 md5 = MD5.Create();
			byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(text));
			
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("x2"));
			}

			return sb.ToString();

		}
	}
}
