using System;
using System.IO;
using System.Text;

namespace Zefie
{
    public class Data
    {
        public static int BlockSize = 8192;
        /// <summary>
        /// Converts binary data into its hexadecimal representation
        /// </summary>
        /// <param name="bytes">Data</param>
        /// <returns>A hexidecimal string</returns>
        public static string byteToHex(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        /// <summary>
        /// Converts a hexadecimal string into its binary data representation
        /// </summary>
        /// <param name="hex">Hexadecimal string</param>
        /// <returns>Binary data</returns>
        public static byte[] hexToBytes(string hex)
        {
            int NumberChars = hex.Length / 2;
            byte[] bytes = new byte[NumberChars];
            using (var sr = new StringReader(hex))
            {
                for (int i = 0; i < NumberChars; i++)
                    bytes[i] =
                      Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return bytes;
        }
        /// <summary>
        /// Encodes data into Base64
        /// </summary>
        /// <param name="data">Binary data to encode</param>
        /// <returns>A base64 encoded string</returns>
        public static string base64Encode(byte[] data)
        {
            return Convert.ToBase64String(data);
        }
        /// <summary>
        /// Encodes a string into Base64
        /// </summary>
        /// <param name="text">String to encode</param>
        /// <returns>A base64 encoded string</returns>
        public static string base64Encode(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }
        /// <summary>
        /// Decodes Base64 encoded data
        /// </summary>
        /// <param name="text">Base64 encoded data</param>
        /// <returns>Binary data</returns>
        public static byte[] base64Decode(string text)
        {
            return Convert.FromBase64String(text);
        }
        /// <summary>
        /// Reads data from STDIN
        /// </summary>
        /// <returns>Data captured from STDIN</returns>
        public static byte[] readFromStdin()
        {
            using (Stream stdin = Console.OpenStandardInput())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] buffer = new byte[BlockSize];
                    int bytes;
                    while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, bytes);
                    }
                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }
    }
}