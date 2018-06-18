using System.Security.Cryptography;
using System.Text;

namespace BackEndServer.Services.HelperServices
{
    public static class HashingTools
    {
        // Returns a string representing the MD5 Hash of an input string.
        public static string MD5Hash(this string stringToHash)
        {
            MD5 md5_Service = MD5.Create();
            StringBuilder hashBuilder = new StringBuilder();

            byte[] stringInBytes = Encoding.ASCII.GetBytes(stringToHash);
            byte[] hashInBytes = md5_Service.ComputeHash(stringInBytes);

            // Convert bytes of the hash result into a string using a stringbuilder.
            for (int i = 0; i < hashInBytes.Length; i++)
            {
                hashBuilder.Append(hashInBytes[i].ToString("X2"));
            }
            return hashBuilder.ToString();
        }
    }
}