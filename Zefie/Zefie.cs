using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Zefie
{
    public class Bemani
    {
        /// <summary>
        /// Generates a PSun compatible eAmuse card code
        /// </summary>
        public static string eAmuseCardGen()
        {
            return "E004" + Strings.genHexString(12);
        }
    }
    public class Crypto
    {

        //Preconfigured Encryption Parameters
        public static readonly int BlockBitSize = 128;
        public static readonly int KeyBitSize = 256;

        //Preconfigured Password Key Derivation Parameters
        public static readonly int SaltBitSize = 64;
        public static readonly int Iterations = 10000;
        public static readonly int MinPasswordLength = 12;
        
        /// <summary>
        /// Generates random bytes via RNGCryptoServiceProvider
        /// </summary>
        /// <param name="length">Number of bytes to generate</param>
        /// <returns>Random binary data</returns>
        public static byte[] genCryptoBytes(int length = 4)
        {
            byte[] data = new byte[length];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(data);
            }
            return data;
        }
        /// <summary>
        /// Generates a random number via RNGCryptoServiceProvider
        /// </summary>
        /// <param name="min">Minimum number to generate</param>
        /// <param name="max">Maximum number to generate</param>
        /// <returns>A random number between <paramref name="min"/> and <paramref name="max"/></returns>
        public static int genCryptoNumber(int min = 0, int max = Int32.MaxValue)
        {
            if (min > max) throw new ArgumentOutOfRangeException("min should not be greater than max");
            if (min == max) return min;
            long diff = (long)max - min;

            while (true)
            {
                byte[] uint32Buffer = genCryptoBytes();
                uint rand = BitConverter.ToUInt32(uint32Buffer, 0);
                const long maxv = (1 + (long)int.MaxValue);
                long remainder = maxv % diff;
                if (rand < maxv - remainder)
                {
                    return (int)(min + (rand % diff));
                }
            }
        }
        /// <summary>
        /// Generates a random string via RNGCryptoServiceProvider
        /// </summary>
        /// <param name="length">Number of characters</param>
        /// <param name="chars">Characters to use in generation</param>
        /// <returns>A random string of characters</returns>
        public static string genCryptoString(int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            char[] AvailableCharacters = chars.ToCharArray();
            char[] identifier = new char[length];
            byte[] randomData = genCryptoBytes(length);

            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            return new string(identifier);
        }
        /// <summary>
        /// Generates a random string via RNGCryptoServiceProvider
        /// </summary>
        /// <param name="length">Number of characters</param>
        /// <param name="append">Characters to append to the default set of characters</param>
        /// <returns>A random string of characters</returns>
        public static string genCryptoString(int length, char[] append)
        {
            return Crypto.genCryptoString(length, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" + new String(append));
        }
        /// <summary>
        /// Generates a hexadecimal string via RNGCryptoServiceProvider
        /// </summary>
        /// <param name="bits">length in bits</param>
        /// <returns>A hexadecimal string</returns>
        public static string genHash(int bits = 256)
        {
            return Crypto.genCryptoString((bits / 8), "0123456789ABCDEF");
        }
        /// <summary>
        /// Generates a key using getCryptoBytes
        /// </summary>
        /// <param name="bits">length in bits</param>
        /// <returns>Random binary data</returns>
        public static byte[] genCryptoKey(int bits = 256)
        {
            return Crypto.genCryptoBytes(bits / 8);
        }
        /// <summary>
        /// Encrypts a string via AES
        /// </summary>
        /// <param name="toEncrypt">String to encrypt</param>
        /// <param name="cryptKey">First encryption key</param>
        /// <param name="authKey">Second encryption key</param>
        /// <param name="nonSecretPayload">Salt payload</param>
        /// <returns>Base64 encoded encrypted data</returns>
        public static string encrypt(string toEncrypt, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null)
        {
            if (string.IsNullOrEmpty(toEncrypt))
                throw new ArgumentException("What are we encrypting?", "toEncrypt");

            var plainText = Encoding.UTF8.GetBytes(toEncrypt);
            var cipherText = encrypt(plainText, cryptKey, authKey, nonSecretPayload);
            return Convert.ToBase64String(cipherText);
        }
        /// <summary>
        /// Encrypts data via AES
        /// </summary>
        /// <param name="toEncrypt">Data to encrypt</param>
        /// <param name="cryptKey">First encryption key</param>
        /// <param name="authKey">Second encryption key</param>
        /// <param name="nonSecretPayload">Salt payload</param>
        /// <returns>Encrypted binary data</returns>
        public static byte[] encrypt(byte[] toEncrypt, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null)
        {
            //User Error Checks
            if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
                throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "cryptKey");

            if (authKey == null || authKey.Length != KeyBitSize / 8)
                throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "authKey");

            if (toEncrypt == null || toEncrypt.Length < 1)
                throw new ArgumentException("What are we encrypting?", "toEncrypt");

            //non-secret payload optional
            nonSecretPayload = nonSecretPayload ?? new byte[] { };

            byte[] cipherText;
            byte[] iv;

            using (var aes = new AesManaged
            {
                KeySize = KeyBitSize,
                BlockSize = BlockBitSize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {

                //Use random IV
                aes.GenerateIV();
                iv = aes.IV;

                using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
                using (var cipherStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cryptoStream))
                    {
                        //Encrypt Data
                        binaryWriter.Write(toEncrypt);
                    }

                    cipherText = cipherStream.ToArray();
                }

            }

            //Assemble encrypted message and add authentication
            using (var hmac = new HMACSHA256(authKey))
            using (var encryptedStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(encryptedStream))
                {
                    //Prepend non-secret payload if any
                    binaryWriter.Write(nonSecretPayload);
                    //Prepend IV
                    binaryWriter.Write(iv);
                    //Write Ciphertext
                    binaryWriter.Write(cipherText);
                    binaryWriter.Flush();

                    //Authenticate all data
                    var tag = hmac.ComputeHash(encryptedStream.ToArray());
                    //Postpend tag
                    binaryWriter.Write(tag);
                }
                return encryptedStream.ToArray();
            }

        }
        /// <summary>
        /// Encrypts a string via AES
        /// </summary>
        /// <param name="toEncrypt">String to encrypt</param>
        /// <param name="password">Encryption key string</param>
        /// <param name="nonSecretPayload">Salt payload</param>
        /// <returns>Base64 encoded encrypted data</returns>
        public static string encrypt(string toEncrypt, string password, byte[] nonSecretPayload = null)
        {
            if (string.IsNullOrEmpty(toEncrypt))
                throw new ArgumentException("What are we encrpyting?", "toEncrypt");

            var plainText = Encoding.UTF8.GetBytes(toEncrypt);
            var cipherText = encrypt(plainText, password, nonSecretPayload);
            return Convert.ToBase64String(cipherText);
        }
        /// <summary>
        /// Encrypts data via AES
        /// </summary>
        /// <param name="toEncrypt">Data to encrypt</param>
        /// <param name="password">Encryption key string</param>
        /// <param name="nonSecretPayload">Salt payload</param>
        /// <returns>Encrypted binary data</returns>
        public static byte[] encrypt(byte[] toEncrypt, string password, byte[] nonSecretPayload = null)
        {
            nonSecretPayload = nonSecretPayload ?? new byte[] { };

            //User Error Checks
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

            if (toEncrypt == null || toEncrypt.Length == 0)
                throw new ArgumentException("What are we encrypting?", "secretMessage");

            if (password.Length < MinPasswordLength)
            {
                while (password.Length < MinPasswordLength)
                    password = password + "\0";
            }


            var payload = new byte[((SaltBitSize / 8) * 2) + nonSecretPayload.Length];

            Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
            int payloadIndex = nonSecretPayload.Length;

            byte[] cryptKey;
            byte[] authKey;
            //Use Random Salt to prevent pre-generated weak password attacks.
            using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize / 8, Iterations))
            {
                var salt = generator.Salt;

                //Generate Keys
                cryptKey = generator.GetBytes(KeyBitSize / 8);

                //Create Non Secret Payload
                Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
                payloadIndex += salt.Length;
            }

            //Deriving separate key, might be less efficient than using HKDF, 
            //but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
            using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize / 8, Iterations))
            {
                var salt = generator.Salt;

                //Generate Keys
                authKey = generator.GetBytes(KeyBitSize / 8);

                //Create Rest of Non Secret Payload
                Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
            }

            return encrypt(toEncrypt, cryptKey, authKey, payload);
        }
        /// <summary>
        /// Decrypts an AES encrypted string
        /// </summary>
        /// <param name="toDecrypt">Base64 encoded encrypted data</param>
        /// <param name="cryptKey">First encryption key</param>
        /// <param name="authKey">Second encryption key</param>
        /// <param name="nonSecretPayloadLength">Salt payload</param>
        /// <returns>Decrypted string</returns>
        public static string decrypt(string toDecrypt, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
        {
            if (string.IsNullOrWhiteSpace(toDecrypt))
                throw new ArgumentException("What are we decrypting?", "encryptedMessage");

            var cipherText = Convert.FromBase64String(toDecrypt);
            var plainText = decrypt(cipherText, cryptKey, authKey, nonSecretPayloadLength);
            return Encoding.UTF8.GetString(plainText);
        }
        /// <summary>
        /// Decrypts AES encrypted binary data
        /// </summary>
        /// <param name="toDecrypt">Data to decrypt</param>
        /// <param name="cryptKey">First encryption key</param>
        /// <param name="authKey">Second encryption key</param>
        /// <param name="nonSecretPayloadLength">Salt payload</param>
        /// <returns>Decrypted binary data</returns>
        public static byte[] decrypt(byte[] toDecrypt, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
        {

            //Basic Usage Error Checks
            if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
                throw new ArgumentException(String.Format("CryptKey needs to be {0} bit!", KeyBitSize), "cryptKey");

            if (authKey == null || authKey.Length != KeyBitSize / 8)
                throw new ArgumentException(String.Format("AuthKey needs to be {0} bit!", KeyBitSize), "authKey");

            if (toDecrypt == null || toDecrypt.Length == 0)
                throw new ArgumentException("What are we decrypting?", "encryptedMessage");

            using (var hmac = new HMACSHA256(authKey))
            {
                var sentTag = new byte[hmac.HashSize / 8];
                //Calculate Tag
                var calcTag = hmac.ComputeHash(toDecrypt, 0, toDecrypt.Length - sentTag.Length);
                var ivLength = (BlockBitSize / 8);

                //if message length is to small just return null
                if (toDecrypt.Length < sentTag.Length + nonSecretPayloadLength + ivLength)
                    return null;

                //Grab Sent Tag
                Array.Copy(toDecrypt, toDecrypt.Length - sentTag.Length, sentTag, 0, sentTag.Length);

                //Compare Tag with constant time comparison
                var compare = 0;
                for (var i = 0; i < sentTag.Length; i++)
                    compare |= sentTag[i] ^ calcTag[i];

                //if message doesn't authenticate return null
                if (compare != 0)
                    return null;

                using (var aes = new AesManaged
                {
                    KeySize = KeyBitSize,
                    BlockSize = BlockBitSize,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                })
                {

                    //Grab IV from message
                    var iv = new byte[ivLength];
                    Array.Copy(toDecrypt, nonSecretPayloadLength, iv, 0, iv.Length);

                    using (var decrypter = aes.CreateDecryptor(cryptKey, iv))
                    using (var plainTextStream = new MemoryStream())
                    {
                        using (var decrypterStream = new CryptoStream(plainTextStream, decrypter, CryptoStreamMode.Write))
                        using (var binaryWriter = new BinaryWriter(decrypterStream))
                        {
                            //Decrypt Cipher Text from Message
                            binaryWriter.Write(
                                toDecrypt,
                                nonSecretPayloadLength + iv.Length,
                                toDecrypt.Length - nonSecretPayloadLength - iv.Length - sentTag.Length
                            );
                        }
                        //Return Plain Text
                        return plainTextStream.ToArray();
                    }
                }
            }
        }
        /// <summary>
        /// Decrypts an AES encrypted string
        /// </summary>
        /// <param name="toDecrypt">Base64 encoded encrypted data</param>
        /// <param name="password">Encryption key string</param>
        /// <param name="nonSecretPayloadLength">Salt payload</param>
        /// <returns>Decrypted string</returns>
        public static string decrypt(string toDecrypt, string password, int nonSecretPayloadLength = 0)
        {
            if (string.IsNullOrWhiteSpace(toDecrypt))
                throw new ArgumentException("What are we decrypting?", "encryptedMessage");

            var cipherText = Convert.FromBase64String(toDecrypt);
            var plainText = decrypt(cipherText, password, nonSecretPayloadLength);
            return Encoding.UTF8.GetString(plainText);
        }
        /// <summary>
        /// Decrypts AES encrypted binary data
        /// </summary>
        /// <param name="toDecrypt">Base64 encoded encrypted data</param>
        /// <param name="password">Encryption key string</param>
        /// <param name="nonSecretPayloadLength">Salt payload</param>
        /// <returns>Decrypted binary data</returns>
        public static byte[] decrypt(byte[] toDecrypt, string password, int nonSecretPayloadLength = 0)
        {
            //User Error Checks
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

            if (toDecrypt == null || toDecrypt.Length == 0)
                throw new ArgumentException("What are we decrypting?!", "encryptedMessage");

            if (password.Length < MinPasswordLength)
            {
                while (password.Length < MinPasswordLength)
                    password = password + "\0";
            }

            var cryptSalt = new byte[SaltBitSize / 8];
            var authSalt = new byte[SaltBitSize / 8];

            //Grab Salt from Non-Secret Payload
            Array.Copy(toDecrypt, nonSecretPayloadLength, cryptSalt, 0, cryptSalt.Length);
            Array.Copy(toDecrypt, nonSecretPayloadLength + cryptSalt.Length, authSalt, 0, authSalt.Length);

            byte[] cryptKey;
            byte[] authKey;

            //Generate crypt key
            using (var generator = new Rfc2898DeriveBytes(password, cryptSalt, Iterations))
            {
                cryptKey = generator.GetBytes(KeyBitSize / 8);
            }
            //Generate auth key
            using (var generator = new Rfc2898DeriveBytes(password, authSalt, Iterations))
            {
                authKey = generator.GetBytes(KeyBitSize / 8);
            }

            return decrypt(toDecrypt, cryptKey, authKey, cryptSalt.Length + authSalt.Length + nonSecretPayloadLength);
        }
        /// <summary>
        /// Encrypts binary data and saves it to a file
        /// </summary>
        /// <param name="filename">Output file</param>
        /// <param name="toEncrypt">Data to encrypt</param>
        /// <param name="cryptKey">First encryption key</param>
        /// <param name="authKey">Second encryption key</param>
        /// <param name="nonSecretPayloadLength">Salt payload</param>
        public static void encryptToFile(string filename, byte[] toEncrypt, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null)
        {
            byte[] data = encrypt(toEncrypt, cryptKey, authKey, nonSecretPayload);
            FileStream f = File.OpenWrite(filename);
            f.Write(data, 0, data.Length);
            f.Close();
        }
        /// <summary>
        /// Encrypts binary data and saves it to a file
        /// </summary>
        /// <param name="filename">Output file</param>
        /// <param name="toEncrypt">Data to encrypt</param>
        /// <param name="password">Encryption key string</param>
        /// <param name="nonSecretPayload">Salt payload</param>
        public static void encryptToFile(string filename, byte[] toEncrypt, string password, byte[] nonSecretPayload = null)
        {
            byte[] data = encrypt(toEncrypt, password, nonSecretPayload);
            FileStream f = File.OpenWrite(filename);
            f.Write(data, 0, data.Length);
            f.Close();
        }
        /// <summary>
        /// Encrypts a string via AES and saves it to a file
        /// </summary>
        /// <param name="filename">Output file</param>
        /// <param name="toEncrypt">String to encrypt</param>
        /// <param name="password">Encryption key string</param>
        /// <param name="nonSecretPayload">Salt payload</param>
        public static void encryptToFile(string filename, string toEncrypt, string password, byte[] nonSecretPayload = null)
        {
            byte[] data = Convert.FromBase64String(encrypt(toEncrypt, password, nonSecretPayload));
            FileStream f = File.OpenWrite(filename);
            f.Write(data, 0, data.Length);
            f.Close();
        }

        /// <summary>
        /// Encrypts a string via AES and saves it to a file
        /// </summary>
        /// <param name="filename">Outfile file</param>
        /// <param name="toEncrypt">String to encrypt</param>
        /// <param name="cryptKey">First encryption key</param>
        /// <param name="authKey">Second encryption key</param>
        /// <param name="nonSecretPayloadLength">Salt payload</param>
        public static void encryptToFile(string filename, string toEncrypt, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null)
        {
            encryptToFile(filename, Encoding.UTF8.GetBytes(toEncrypt), cryptKey, authKey);
        }
        /// <summary>
        /// Decrypts data from an AES encrypted file
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <param name="cryptKey">First encryption key</param>
        /// <param name="authKey">Second encryption key</param>
        /// <param name="nonSecretPayloadLength">Salt payload</param>
        /// <returns>Decrypted data</returns>
        public static byte[] decryptFromFile(string filename, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
        {
            FileStream f = File.OpenRead(filename);
            byte[] data = new byte[f.Length];
            f.Read(data, 0, (int)f.Length);
            f.Close();
            return decrypt(data, cryptKey, authKey, nonSecretPayloadLength);
        }
        /// <summary>
        /// Decrypts data from an AES encrypted file
        /// </summary>
        /// <param name="filename">Input file</param>
        /// <param name="cryptKey">First encryption key</param>
        /// <param name="authKey">Second encryption key</param>
        /// <param name="password">Encryption key string</param>
        /// <param name="nonSecretPayloadLength">Salt payload</param>
        /// <returns>Decrypted data</returns>
        public static byte[] decryptFromFile(string filename, string password, int nonSecretPayloadLength = 0)
        {
            FileStream f = File.OpenRead(filename);
            byte[] data = new byte[f.Length];
            f.Read(data, 0, (int)f.Length);
            f.Close();
            return decrypt(data, password, nonSecretPayloadLength);
        }

    }
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
        public class Hashing
        {
            /// <summary>
            /// Computes a SHA512 hash
            /// </summary>
            /// <param name="data">Data to hash</param>
            /// <returns>Hexadecimal SHA512 hash string</returns>
            public static string SHA512(byte[] data)
            {
                using (SHA512 shaM = new SHA512Managed())
                    return byteToHex(shaM.ComputeHash(data));
            }
            /// <summary>
            /// Computes a SHA512 hash
            /// </summary>
            /// <param name="text">String to hash</param>
            /// <returns>Hexadecimal SHA512 hash string</returns>
            public static string SHA512(string text)
            {
                return SHA512(Encoding.UTF8.GetBytes(text));
            }
            /// <summary>
            /// Computes a SHA384 hash
            /// </summary>
            /// <param name="data">Data to hash</param>
            /// <returns>Hexadecimal SHA384 hash string</returns>
            public static string SHA384(byte[] data)
            {
                using (SHA384 shaM = new SHA384Managed())
                    return byteToHex(shaM.ComputeHash(data));
            }
            /// <summary>
            /// Computes a SHA384 hash
            /// </summary>
            /// <param name="text">String to hash</param>
            /// <returns>Hexadecimal SHA384 hash string</returns>
            public static string SHA384(string text)
            {
                return SHA384(Encoding.UTF8.GetBytes(text));
            }
            /// <summary>
            /// Computes a SHA256 hash
            /// </summary>
            /// <param name="data">Data to hash</param>
            /// <returns>Hexadecimal SHA256 hash string</returns>
            public static string SHA256(byte[] data)
            {
                using (SHA256 shaM = new SHA256Managed())
                    return byteToHex(shaM.ComputeHash(data));
            }
            /// <summary>
            /// Computes a SHA256 hash
            /// </summary>
            /// <param name="text">String to hash</param>
            /// <returns>Hexadecimal SHA256 hash string</returns>
            public static string SHA256(string text)
            {
                return SHA256(Encoding.UTF8.GetBytes(text));
            }
            /// <summary>
            /// Computes a SHA1 hash
            /// </summary>
            /// <param name="data">Data to hash</param>
            /// <returns>Hexadecimal SHA1 hash string</returns>
            public static string SHA1(byte[] data)
            {
                using (SHA1 shaM = new SHA1Managed())
                    return byteToHex(shaM.ComputeHash(data));
            }
            /// <summary>
            /// Computes a SHA1 hash
            /// </summary>
            /// <param name="text">String to hash</param>
            /// <returns>Hexadecimal SHA1 hash string</returns>
            public static string SHA1(string text)
            {
                return SHA1(Encoding.UTF8.GetBytes(text));
            }
            /// <summary>
            /// Computes a MD5 hash
            /// </summary>
            /// <param name="data">Data to hash</param>
            /// <returns>Hexadecimal MD5 hash string</returns>
            public static string MD5(byte[] data)
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                    return byteToHex(md5.ComputeHash(data));
            }
            /// <summary>
            /// Computes a MD5 hash
            /// </summary>
            /// <param name="text">String to hash</param>
            /// <returns>Hexadecimal MD5 hash string</returns>
            public static string MD5(string text)
            {
                return MD5(Encoding.UTF8.GetBytes(text));
            }
            /// <summary>
            /// Computes the MD5 hash of the file
            /// </summary>
            /// <param name="filename">File to hash</param>
            /// <returns>Hexadecimal MD5 hash string</returns>
            public static string MD5File(string filename)
            {
                int offset = 0;
                byte[] block = new byte[BlockSize];
                byte[] hash;
                using (var f = new BufferedStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    using (MD5 md5 = new MD5CryptoServiceProvider())
                    {
                        // For each block:
                        while (offset + block.Length < f.Length)
                        {
                            f.Position = offset;
                            f.Read(block, 0, BlockSize);
                            offset += md5.TransformBlock(block, 0, block.Length, null, 0);
                        }
                        int remain = (int)(f.Length - (long)offset);
                        block = new byte[remain];
                        f.Position = offset;
                        f.Read(block, 0, remain);
                        md5.TransformFinalBlock(block, 0, block.Length);
                        hash = md5.Hash;
                    }
                }
                return byteToHex(hash);
            }
            /// <summary>
            /// Computes the SHA1 of the file
            /// </summary>
            /// <param name="filename">File to hash</param>
            /// <returns>Hexadecimal SHA1 hash string</returns>
            public static string SHA1File(string filename)
            {
                int offset = 0;
                byte[] block = new byte[BlockSize];
                byte[] hash;
                using (var f = new BufferedStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    using (SHA1 shaM = new SHA1Managed())
                    {
                        // For each block:
                        while (offset + block.Length < f.Length)
                        {
                            f.Position = offset;
                            f.Read(block, 0, BlockSize);
                            offset += shaM.TransformBlock(block, 0, block.Length, null, 0);
                        }
                        int remain = (int)(f.Length - (long)offset);
                        block = new byte[remain];
                        f.Position = offset;
                        f.Read(block, 0, remain);
                        shaM.TransformFinalBlock(block, 0, block.Length);
                        hash = shaM.Hash;
                    }
                }
                return byteToHex(hash);
            }
            /// <summary>
            /// Computes the SHA256 hash of the file
            /// </summary>
            /// <param name="filename">File to hash</param>
            /// <returns>Hexadecimal SHA256 hash string</returns>
            public static string SHA256File(string filename)
            {
                int offset = 0;
                byte[] block = new byte[BlockSize];
                byte[] hash;
                using (var f = new BufferedStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    using (SHA256 shaM = new SHA256Managed())
                    {
                        // For each block:
                        while (offset + block.Length < f.Length)
                        {
                            f.Position = offset;
                            f.Read(block, 0, BlockSize);
                            offset += shaM.TransformBlock(block, 0, block.Length, null, 0);
                        }
                        int remain = (int)(f.Length - (long)offset);
                        block = new byte[remain];
                        f.Position = offset;
                        f.Read(block, 0, remain);
                        shaM.TransformFinalBlock(block, 0, block.Length);
                        hash = shaM.Hash;
                    }
                }
                return byteToHex(hash);
            }
            /// <summary>
            /// Computes the SHA384 hash of the file
            /// </summary>
            /// <param name="filename">File to hash</param>
            /// <returns>Hexadecimal SHA384 hash string</returns>
            public static string SHA384File(string filename)
            {
                int offset = 0;
                byte[] block = new byte[BlockSize];
                byte[] hash;
                using (var f = new BufferedStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    using (SHA384 shaM = new SHA384Managed())
                    {
                        // For each block:
                        while (offset + block.Length < f.Length)
                        {
                            f.Position = offset;
                            f.Read(block, 0, BlockSize);
                            offset += shaM.TransformBlock(block, 0, block.Length, null, 0);
                        }
                        int remain = (int)(f.Length - (long)offset);
                        block = new byte[remain];
                        f.Position = offset;
                        f.Read(block, 0, remain);
                        shaM.TransformFinalBlock(block, 0, block.Length);
                        hash = shaM.Hash;
                    }
                }
                return byteToHex(hash);
            }
            /// <summary>
            /// Computes the SHA512 hash of the file
            /// </summary>
            /// <param name="filename">File to hash</param>
            /// <returns>Hexadecimal SHA512 hash string</returns>
            public static string SHA512File(string filename)
            {
                int offset = 0;
                byte[] block = new byte[BlockSize];
                byte[] hash;
                using (var f = new BufferedStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    using (SHA512 shaM = new SHA512Managed())
                    {
                        // For each block:
                        while (offset + block.Length < f.Length)
                        {
                            f.Position = offset;
                            f.Read(block, 0, BlockSize);
                            offset += shaM.TransformBlock(block, 0, block.Length, null, 0);
                        }
                        int remain = (int)(f.Length - (long)offset);
                        block = new byte[remain];
                        f.Position = offset;
                        f.Read(block, 0, remain);
                        shaM.TransformFinalBlock(block, 0, block.Length);
                        hash = shaM.Hash;
                    }
                }
                return byteToHex(hash);
            }

        }
    }
    public class Imaging
    {
        /// <summary>
        /// Inverts the colors of the bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>Inverted Bitmap</returns>
        public static Bitmap invertColors(Bitmap bitmap)
        {
            var bitmapRead = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
            var bitmapLength = bitmapRead.Stride * bitmapRead.Height;
            var bitmapBGRA = new byte[bitmapLength];
            Marshal.Copy(bitmapRead.Scan0, bitmapBGRA, 0, bitmapLength);
            bitmap.UnlockBits(bitmapRead);

            for (int i = 0; i < bitmapLength; i += 4)
            {
                bitmapBGRA[i] = (byte)(255 - bitmapBGRA[i]);
                bitmapBGRA[i + 1] = (byte)(255 - bitmapBGRA[i + 1]);
                bitmapBGRA[i + 2] = (byte)(255 - bitmapBGRA[i + 2]);
                //        [i + 3] = ALPHA.
            }
            Bitmap outBMP = new Bitmap(bitmap);
            bitmap = null;
            var bitmapWrite = outBMP.LockBits(new Rectangle(0, 0, outBMP.Width, outBMP.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            Marshal.Copy(bitmapBGRA, 0, bitmapWrite.Scan0, bitmapLength);
            outBMP.UnlockBits(bitmapWrite);
            return outBMP;
        }
        /// <summary>
        /// Inverts the colors of the image
        /// </summary>
        /// <param name="image"></param>
        /// <returns>Inverted Bitmap</returns>
        public static Image invertColors(Image image)
        {
            Bitmap bitmap = new Bitmap(image);
            bitmap = invertColors(bitmap);
            return (Image)bitmap;
        }
        /// <summary>
        /// Scales the image to the specified size
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Scaled image</returns>
        public static Image Scale(Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
            //set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return (Image)result;
        }
        /// <summary>
        /// Scales the image to the specified percent of the original size
        /// </summary>
        /// <param name="image"></param>
        /// <param name="percent"></param>
        /// <returns>Scaled image</returns>
        public static Image Scale(Image image, double percent)
        {
            int width = (int)Math.calcPercentOf((double)image.Width, percent);
            int height = (int)Math.calcPercentOf((double)image.Height, percent);
            return Scale(image, width, height);
        }
        /// <summary>
        /// Scales the image to the specified width, while retaining aspect ratio
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <returns>Scaled image</returns>
        public static Image Scale(Image image, int width)
        {
            double percent = Math.calcPercent(width, image.Width);
            int height = (int)Math.calcPercentOf((double)image.Height, percent);
            return Scale(image, width, height);
        }
        /// <summary>
        /// Scales the bitmap the specified size
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>Scaled bitmap</returns>
        public static Bitmap Scale(Bitmap bitmap, int width, int height)
        {
            Image image = (Image)bitmap;
            return new Bitmap(Scale(image, width, height));
        }
        /// <summary>
        /// Scales the bitmap to the specified percent of the original size
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="percent"></param>
        /// <returns>Scaled bitmap</returns>
        public static Bitmap Scale(Bitmap bitmap, double percent)
        {
            Image image = (Image)bitmap;
            int width = (int)Math.calcPercentOf((double)image.Width, percent);
            int height = (int)Math.calcPercentOf((double)image.Height, percent);
            return new Bitmap(Scale(image, width, height));
        }
        /// <summary>
        /// Scales the bitmap to the specified width, while retaining aspect ratio
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="width"></param>
        /// <returns>Scaled bitmap</returns>
        public static Bitmap Scale(Bitmap bitmap, int width)
        {
            Image image = (Image)bitmap;
            double percent = Math.calcPercent(width, image.Width);
            int height = (int)Math.calcPercentOf((double)image.Height, percent);
            return new Bitmap(Scale(image, width, height));
        }
    }
    public class Networking
    {
        /// <summary>
        /// Checks if the specified port is in use
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address">IPAdddress, defaults to localhost</param>
        /// <returns>True if the port is avaible, false if it is in use</returns>
        public static bool isPortAvailable(int port, IPAddress address = null)
        {
            // 127.0.0.1
            if (address == null)
                address = new IPAddress(16777343);

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Address == address && tcpi.LocalEndPoint.Port == port)
                {
                    return false;
                }
            }
            return true;
        }
    }
    public class Path
    {
        /// <summary>
        /// Finds the drive letter of the drive with the specified label
        /// </summary>
        /// <param name="label">Label of the drive to search for (case sensitive)</param>
        /// <returns>Drive letter in format of X:\, or null if no results</returns>
        public static string getDriveLetterFromLabel(string label)
        {
            foreach (DriveInfo DI in DriveInfo.GetDrives())
            {
                try
                {
                    if (DI.VolumeLabel.Length > 0)
                    {
                        if (DI.VolumeLabel == label)
                        {
                            return DI.Name;
                        }
                    }
                }
                catch { }
            }
            return null;
        }
    }
    public class Prompts
    {
        /// <summary>
        /// Displays a Yes/No prompt
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="caption">Title of the dialog</param>
        /// <returns>User's choice in boolean value</returns>
        public static bool ShowConfirm(string text, string caption = "")
        {
            Form prompt = new Form();
            prompt.Text = caption;
            prompt.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            prompt.MaximizeBox = false;
            prompt.MinimizeBox = false;
            prompt.ShowInTaskbar = false;
            bool result = false;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            Label textLabel = new Label() { Left = 17, Top = 10, AutoSize = true, MaximumSize = new System.Drawing.Size(550, 1000), Text = text };
            Size labelSize = new Size();
            using (Graphics g = textLabel.CreateGraphics())
            {
                SizeF size = g.MeasureString(text, textLabel.Font, (textLabel.MaximumSize.Width + 35));
                labelSize.Height = (int)System.Math.Ceiling(size.Height);
                labelSize.Width = (int)System.Math.Ceiling(size.Width);
            }
            prompt.Height = labelSize.Height + 90;
            prompt.Width = labelSize.Width + 45;
            Button cancel = new Button() { Text = "No", Left = -2000, Width = 35, Top = (labelSize.Height + 20) };
            cancel.Left = (((labelSize.Width + textLabel.Left) - cancel.Width) - 5);
            Button confirmation = new Button() { Text = "Yes", Left = -2000, Width = 35, Top = cancel.Top };
            confirmation.Left = (((cancel.Left) - confirmation.Width) - 1);
            confirmation.Click += (sender, e) => { result = true; prompt.Close(); };
            cancel.Click += (sender, e) => { result = false; prompt.Close(); };
            prompt.KeyDown += (sender, e) => { if (e.KeyCode == Keys.Escape) { cancel.PerformClick(); } };
            confirmation.KeyDown += (sender, e) => { if (e.KeyCode == Keys.Escape) { cancel.PerformClick(); } };
            cancel.KeyDown += (sender, e) => { if (e.KeyCode == Keys.Escape) { cancel.PerformClick(); } };
            cancel.TabIndex = 1;
            confirmation.TabIndex = 2;
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.ShowDialog();
            return result;
        }
        /// <summary>
        /// Displays a prompt, allowing the user to enter text.
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="caption">Title of the dialog</param>
        /// <returns>The text the user entered</returns>
        public static string ShowPrompt(string text, string caption = "")
        {
            Form prompt = new Form();
            prompt.Text = caption;
            prompt.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            prompt.MaximizeBox = false;
            prompt.MinimizeBox = false;
            prompt.ShowInTaskbar = false;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            Label textLabel = new Label() { Left = 17, Top = 10, AutoSize = true, MaximumSize = new System.Drawing.Size(550, 1000), Text = text };
            Size labelSize = new Size();
            using (Graphics g = textLabel.CreateGraphics())
            {
                SizeF size = g.MeasureString(text, textLabel.Font, (textLabel.MaximumSize.Width + 35));
                labelSize.Height = (int)System.Math.Ceiling(size.Height);
                labelSize.Width = (int)System.Math.Ceiling(size.Width);
            }
            prompt.Height = labelSize.Height + 115;
            prompt.Width = labelSize.Width + 50;
            TextBox textBox = new TextBox() { Left = 20, Top = (labelSize.Height + 15), Width = (labelSize.Width) };
            Button confirmation = new Button() { Text = "Ok", Left = -2000, Width = 35, Top = ((textBox.Top + textBox.Height) + 5) };
            Button cancel = new Button() { Text = "Cancel", Left = -2000, Width = 60, Top = confirmation.Top };
            cancel.Left = (textBox.Width + textBox.Left) - cancel.Width;
            confirmation.Left = ((cancel.Left) - confirmation.Width) - 1;
            confirmation.Click += (sender, e) => { prompt.Close(); };
            textBox.TabIndex = 1;
            confirmation.TabIndex = 2;
            cancel.TabIndex = 3;
            cancel.Click += (sender, e) => { textBox.Text = ""; prompt.Close(); };
            prompt.KeyDown += (sender, e) => { if (e.KeyCode == Keys.Escape) { cancel.PerformClick(); } };
            textBox.KeyDown += (sender, e) => { if (e.KeyCode == Keys.Escape) { cancel.PerformClick(); } if (e.KeyCode == Keys.Enter) { confirmation.PerformClick(); } };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.ShowDialog();
            return textBox.Text;
        }
        /// <summary>
        /// Shows a MessageBox with an error icon and an OK button
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="caption">Title of the dialog</param>
        public static void ShowError(string text, string caption = "Error")
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// Displays a MessageBox with an information icon and an OK button
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="caption">Title of the dialog</param>
        public static void ShowMsg(string text, string caption = "Message")
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// Displays an OpenFileDialog, prompting the user to select a file
        /// </summary>
        /// <param name="title">Title of the dialog</param>
        /// <param name="start_folder">Directory to start in</param>
        /// <param name="filter">File filter</param>
        /// <returns>User selected file, or null</returns>
        public static string browseOpenFile(string title = null, string start_folder = null, string filter = "All Files (*.*)|*.*")
        {
            OpenFileDialog f = new OpenFileDialog();
            if (start_folder != null)
                f.InitialDirectory = start_folder;
            if (title != null)
                f.Title = title;
            f.Filter = filter;
            f.Multiselect = false;
            f.ShowDialog();
            return f.FileName;
        }
        /// <summary>
        /// Displays an OpenFileDialog, prompting the user to select a file, or multiple files
        /// </summary>
        /// <param name="title">Title of the dialog</param>
        /// <param name="start_folder">Directory to start in</param>
        /// <param name="filter">File filter</param>
        /// <returns>User selected files, or null</returns>
        public static string[] browseOpenFiles(string title = null, string start_folder = null, string filter = "All Files (*.*)|*.*")
        {
            OpenFileDialog f = new OpenFileDialog();
            if (start_folder != null)
                f.InitialDirectory = start_folder;
            if (title != null)
                f.Title = title;
            f.Filter = filter;
            f.Multiselect = true;
            f.ShowDialog();
            return f.FileNames;
        }
        /// <summary>
        /// Displays an SaveFileDialog, prompting the user to select a file
        /// </summary>
        /// <param name="title">Title of the dialog</param>
        /// <param name="start_folder">Directory to start in</param>
        /// <param name="filter">File filter</param>
        /// <returns>User selected file, or null</returns>
        public static string browseSaveFile(string title = null, string start_folder = null, string filter = "All Files (*.*)|*.*")
        {
            SaveFileDialog f = new SaveFileDialog();
            if (start_folder != null)
                f.InitialDirectory = start_folder;
            if (title != null)
                f.Title = title;
            f.Filter = filter;
            f.ShowDialog();
            return f.FileName;
        }
        /// <summary>
        /// Displays a FolderBrowserDialog, prompting the user to select a folder
        /// </summary>
        /// <param name="title">Title of the dialog</param>
        /// <param name="new_folder_button">Show the 'New Folder' button</param>
        /// <returns>User selected folder, or null</returns>
        public static string browseFolder(string title = null, bool new_folder_button = true)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            if (title != null)
                f.Description = title;
            f.ShowNewFolderButton = new_folder_button;
            f.ShowDialog();
            return f.SelectedPath;
        }
    }
    public class Strings
    {
        /// <summary>
        /// Generates a hexadecimal string
        /// </summary>
        /// <param name="length">Number of bytes</param>
        /// <returns>A hexadecimal string</returns>
        public static string genHexString(int length)
        {
            return Crypto.genHash(length * 8);
        }
        /// <summary>
        /// Generates a random string
        /// </summary>
        /// <param name="length">Number of characters</param>
        /// <param name="chars">Characters to use in generation</param>
        /// <returns>A random string of characters</returns>
        public static string genString(int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            var random = new Random(Crypto.genCryptoNumber());
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
    public class Math
    {
        /// <summary>
        /// Calculates what percentage (<paramref name="value"/>) is of (<paramref name="max"/>)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <returns>Percentage</returns>
        public static double calcPercent(double value, double max)
        {
            return ((value / max) * 100);
        }
        /// <summary>
        /// Calculates what (<paramref name="value"/>) is (<paramref name="percent"/>) percent of
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percent"></param>
        /// <returns>The full value that (<paramref name="value"/>) is (<paramref name="percent"/>) percent of</returns>
        public static double calcPercentOf(double value, double percent)
        {
            return ((value/100) * percent);
        }
        /// <summary>
        /// Generates a random number
        /// </summary>
        /// <param name="max">Highest number to generate</param>
        /// <returns>A random number</returns>
        public static int random(int max)
        {
            Random rand = new Random(Crypto.genCryptoNumber());
            return rand.Next(max);
        }
        /// <summary>
        /// Generates a random number
        /// </summary>
        /// <param name="min">Lowest number to generate</param>
        /// <param name="max">Highest number to generate</param>
        /// <returns>A random number</returns>
        public static int random(int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException("min should not be greater than max");
            if (min == max) return min;
            Random rand = new Random(new Random().Next(min, max));
            return rand.Next(min, max);
        }
        /// <summary>
        /// Calculates user-friendly interpretation of a file size
        /// </summary>
        /// <param name="value">File size in bytes</param>
        /// <returns>User-friendly interpretation of the file size</returns>
        public static string calcBytes(Int64 value)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (value < 0) { return "-" + calcBytes(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)System.Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
    public class TreeNode
    {
        /// <summary>
        /// Creates nodes and child nodes based on a string path
        /// </summary>
        /// <param name="n">The node create children on</param>
        /// <param name="path">TreeNode.FullPath-style string</param>
        /// <returns>The last child node created</returns>
        public static string createNodesByPath(System.Windows.Forms.TreeNode n, string path)
        {
            List<String> p = new List<string>();
            foreach (string s in path.Split('\\'))
                p.Add(s);

            p.RemoveAt(p.Count - 1);
            for (int i = 0; i < p.Count; i++)
            {
                if (i == 0)
                {
                    if (!n.Nodes.ContainsKey(p[i]))
                    {
                        System.Windows.Forms.TreeNode tmp = new System.Windows.Forms.TreeNode(p[i]);
                        tmp.Name = p[i];
                        n.Nodes.Add(tmp);
                    }
                }
                else
                {
                    if (!n.Nodes.Find(p[i - 1], true)[0].Nodes.ContainsKey(p[i]))
                    {
                        System.Windows.Forms.TreeNode tmp = new System.Windows.Forms.TreeNode(p[i]);
                        tmp.Name = p[i];
                        n.Nodes.Find(p[i - 1], true)[0].Nodes.Add(tmp);
                    }
                }
            }
            return p[p.Count() - 1];
        }
    }
}
