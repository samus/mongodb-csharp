using System;
using System.Security.Cryptography;
using System.Text;

namespace MongoDB.Driver.Util
{
    /// <summary>
    /// 
    /// </summary>
    internal static class MongoHash
    {
        /// <summary>
        /// Generate a hash for the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Generate(string text)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.Default.GetBytes(text));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}