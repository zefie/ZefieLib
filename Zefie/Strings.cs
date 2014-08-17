using System;
using System.Linq;
using System.Text;

namespace Zefie
{
    public class Strings
    {
        /// <summary>
        /// Generates a hexadecimal string
        /// </summary>
        /// <param name="length">Number of bytes</param>
        /// <returns>A hexadecimal string</returns>
        public static string genHexString(int length)
        {
            return Cryptography.genHash(length * 8);
        }
        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="length">Number of characters</param>
        /// <param name="chars">Characters to use in generation</param>
        /// <returns>A random string of characters</returns>
        public static string genString(int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            var random = new Random(Cryptography.genCryptoNumber());
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[Zefie.Math.random(s.Length)])
                          .ToArray());
            return result;
        }
        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="length">Number of characters</param>
        /// <param name="append">Characters to append to the default set of characters</param>
        /// <returns>A random string of characters</returns>
        public static string genString(int length, char[] append)
        {
            return genString(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" + new String(append));
        }
    }
}
