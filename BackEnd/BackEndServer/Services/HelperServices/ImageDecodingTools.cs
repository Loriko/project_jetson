using System;
using System.IO;
using Castle.Core.Internal;

namespace BackEndServer.Services.HelperServices
{
    public class ImageDecodingTools
    {
        public static bool SaveBase64StringToFile(string base64String, string fullPath)
        {
            if (base64String.IsNullOrEmpty() || fullPath.IsNullOrEmpty() || !fullPath.EndsWith(".jpg"))
            {
                return false;
            }

            try
            {
                var bytes = Convert.FromBase64String(base64String);
                using (var imageFile = new FileStream(fullPath, FileMode.Create))
                {
                    imageFile.Write(bytes ,0, bytes.Length);
                    imageFile.Flush();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}