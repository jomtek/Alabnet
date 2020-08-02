using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AlabNet.Screenshare
{
    class Cipher
    {
        public static byte[] Prepend(byte[] bytes, byte[] bytesToPrepend)
        {
            var tmp = new byte[bytes.Length + bytesToPrepend.Length];
            bytesToPrepend.CopyTo(tmp, 0);
            bytes.CopyTo(tmp, bytesToPrepend.Length);
            return tmp;
        }

        public static (byte[] left, byte[] right) Shift(byte[] bytes, int size)
        {
            var left = new byte[size];
            var right = new byte[bytes.Length - size];

            Array.Copy(bytes, 0, left, 0, left.Length);
            Array.Copy(bytes, left.Length, right, 0, right.Length);

            return (left, right);
        }

        public static byte[] Encrypt(byte[] bytesToEncrypt, string password)
        {
            byte[] ivSeed = Guid.NewGuid().ToByteArray();

            var rfc = new Rfc2898DeriveBytes(password, ivSeed);
            byte[] Key = rfc.GetBytes(16);
            byte[] IV = rfc.GetBytes(16);

            byte[] encrypted;
            using (MemoryStream mstream = new MemoryStream())
            {
                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(mstream, aesProvider.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    }
                }
                encrypted = mstream.ToArray();
            }

            var messageLengthAs32Bits = Convert.ToInt32(bytesToEncrypt.Length);
            var messageLength = BitConverter.GetBytes(messageLengthAs32Bits);

            encrypted = Prepend(encrypted, ivSeed);
            encrypted = Prepend(encrypted, messageLength);

            return encrypted;
        }

        public static byte[] Decrypt(byte[] bytesToDecrypt, string password)
        {
            (byte[] messageLengthAs32Bits, byte[] bytesWithIv) = Shift(bytesToDecrypt, 4); // get the message length
            (byte[] ivSeed, byte[] encrypted) = Shift(bytesWithIv, 16);                   // get the initialization vector

            var length = BitConverter.ToInt32(messageLengthAs32Bits, 0);

            var rfc = new Rfc2898DeriveBytes(password, ivSeed);
            byte[] Key = rfc.GetBytes(16);
            byte[] IV = rfc.GetBytes(16);

            using (MemoryStream mStream = new MemoryStream(encrypted))
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider() { Padding = PaddingMode.None })
            using (CryptoStream cryptoStream = new CryptoStream(mStream, aesProvider.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
            {
                cryptoStream.Read(encrypted, 0, length);
                return mStream.ToArray().Take(length).ToArray();
            }
        }
    }
}