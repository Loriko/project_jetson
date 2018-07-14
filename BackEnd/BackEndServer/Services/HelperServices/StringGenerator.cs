using System;
using System.Security.Cryptography;
using System.Text;

namespace BackEndServer.Services.HelperServices
{
    public class StringGenerator
    {
        public static string GenerateRandomString(int minSize, int maxSize)
        {
            // Alphanumerical characters.
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

            // Generate a random number between minimum and maximum size limits.
            Random random = new Random();
            int size = random.Next(minSize, maxSize + 1);


            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[size];
                crypto.GetNonZeroBytes(data);
            }

            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }
    }
}