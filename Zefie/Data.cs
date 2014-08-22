using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Zefie
{
    public class Data
    {
        internal static List<string> _isofiles = new List<string>();
        public static int BlockSize = 8192;
        private static int _isosector = 2048;

        /// <summary>
        /// If true, reads 2352 bytes per sector, otherwise 2048 bytes per sector.
        /// </summary>
        public static bool rawISO9660Mode
        {
            get
            {
                if (_isosector == 2048)
                    return false;
                else
                    return true;
            }
            set
            {
                if (value == true)
                    _isosector = 2352;
                else
                    _isosector = 2048;
            }
        }

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

        public static int readLittleEndianWord(byte[] bytes)
        {
            byte b1 = bytes[0];
            byte b2 = bytes[1];
            byte b3 = bytes[2];
            byte b4 = bytes[3];
            int s = 0;
            s |= b4 & 0xFF;
            s <<= 8;
            s |= b3 & 0xFF;
            s <<= 8;
            s |= b2 & 0xFF;
            s <<= 8;
            s |= b1 & 0xFF;
            return s;
        }


        public static int toInt32BigEndian(byte[] buf, int i)
        {
            return (buf[i] << 24) | (buf[i + 1] << 16) | (buf[i + 2] << 8) | buf[i + 3];
        }

        public static short UValue(byte b)
        {
            return (short)(b & 0xFF);
        }

        public static bool checkPowerOfTwo(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException("number: " + number);
            }
            return (number & number - 1) == 0;
        }


        internal static bool isHeader(byte[] header)
        {
            return (header[1] == 67) && (header[2] == 68) && (header[3] == 48)
            && (header[4] == 48) && (header[5] == 49);
        }
        internal static byte[] copyOfRange(byte[] src, int start, int end)
        {
            int len = end - start;
            byte[] dest = new byte[len];
            // note i is always from 0
            for (int i = 0; i < len; i++)
            {
                dest[i] = src[start + i]; // so 0..n = 0+x..n+x
            }
            return dest;
        }
        internal static bool isPrimaryVolumeDescriptor(byte b)
        {
            return 1 == Zefie.Data.UValue(b);
        }
        internal static int[] getStartAndSize(string file, byte[] data)
        {
            int startSector = readLittleEndianWord(copyOfRange(data, 158, 162));
            int size = readLittleEndianWord(copyOfRange(data, 166, 170));
            return (new int[2] { startSector, size });
        }
        internal static void seekFiles(string file, int[] sectordata, string path = "/")
        {
            try
            {
                FileStream raf = System.IO.File.OpenRead(file);
                byte[] data = new byte[sectordata[1]];
                raf.Position = _isosector * sectordata[0];
                if (rawISO9660Mode)
                    raf.Position += 24;
                raf.Read(data, 0, data.Length);
                raf.Close();
                int count = 0;
                for (int index = 0; index < data.Length; index++)
                {
                    int offset = data[index];
                    if (offset == 0)
                        break;
                    if (count > 1)
                    {
                        parseFile(copyOfRange(data, index, index + offset), file, path);
                        index += offset - 1;
                    }
                    else
                    {
                        count++;
                        index += offset - 1;
                    }
                }
            }
            catch { }
        }
        internal static void parseFile(byte[] data, string file, string path)
        {
            StringBuilder sb = new StringBuilder();
            string test = convertToBinaryString(UValue(data[25]));
            String flags = test.PadLeft(8).Replace(' ', '0');
            bool dir = false;
            if (flags.Substring(6, 1) == "1")
                dir = true;
            int length = UValue(data[32]);
            for (int i = 33; i < 33 + length; i++)
            {
                sb.Append((char)data[i]);
            }
            if ((sb.ToString().Length == 1) && (sb.ToString().Substring(0, 1) == "0"))
                return;
            String nm = dir ? sb.ToString() : sb.ToString().Substring(0, sb.ToString().Length - 2);
            int ss = readLittleEndianWord(copyOfRange(data, 2, 6));
            int sz = readLittleEndianWord(copyOfRange(data, 10, 14));
            if (_isofiles == null)
                _isofiles = new List<string>();
            _isofiles.Add(path + nm);
            if (dir)
                seekFiles(file, new int[] { ss, sz }, "/" + nm + "/");

        }
        
        /// <summary>
        /// Reads a sector from an ISO file
        /// </summary>
        /// <param name="file">ISO file</param>
        /// <param name="nSector">Sector number</param>
        public static void readISO9660Sector(string file, int nSector)
        {
            try
            {
                FileStream raf = System.IO.File.OpenRead(file);
                raf.Position = (_isosector * nSector);
                byte[] sector = new byte[_isosector];
                int[] sd;
                if (rawISO9660Mode)
                    sd = new int[2] { 24, 30 };
                else
                    sd = new int[2] { 0, 6 };

                while (raf.Read(sector, 0, _isosector) > 0)
                {
                    if ((isHeader(copyOfRange(sector, sd[0], sd[1])))
                        && (isPrimaryVolumeDescriptor(sector[sd[0]])))
                    {
                        break;
                    }
                }
                raf.Close();
                if (rawISO9660Mode)
                {
                    byte[] usefulData = copyOfRange(sector, 24, 2072);
                    sector = null;
                    GC.Collect();
                    seekFiles(file, getStartAndSize(file, usefulData));
                }
                else
                {
                    GC.Collect();
                    seekFiles(file, getStartAndSize(file, sector));

                }
            }
            catch { }
        }


        /// <summary>
        /// Converts an integer or byte to binary
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string convertToBinaryString(int x)
        {
            return Convert.ToString(x, 2).PadLeft(8, '0');
        }

        /// <summary>
        /// Read the TOC from an ISO file
        /// </summary>
        /// <param name="file">ISO file</param>
        /// <returns>Directory listing of ISO</returns>
        public static string[] listISO9660Files(string file)
        {
            _isofiles.Clear();
            readISO9660Sector(file,16);
            return _isofiles.ToArray();
        }
    }
}