using System;
using System.IO;

namespace BackEndServer.Services.HelperServices
{
    public static class RootDirectoryTools
    {
        public static string GetWWWRootPhysicalPath()
        {
            string threeParentsLevelUp;
            if (IsLinux)
            {
                threeParentsLevelUp = @"../../../";
            }
            else
            {
                threeParentsLevelUp = @"..\..\..\";
            }
            return Path.Combine
            (
                // "<PROJECT_ROOT>\\BackEnd\\BackEndServer\\bin\\Debug\\netcoreapp2.0\\"
                AppDomain.CurrentDomain.BaseDirectory, 
                // Go up 3 parent levels.
                threeParentsLevelUp,
                // Add the wwwroot child directory of the BackEndServer parent directory.
                @"wwwroot"
            );
        }

        public static string GetWWWRootTempFolderPhysicalPath()
        {
            return Path.Combine(GetWWWRootPhysicalPath(), @"temp");
        }

        public static string GetWWWRootVirtualPathForHTML()
        {
            return @"/";
        }

        public static string GetWWWRootTempFolderVirtualPathForHTML()
        {
            return Path.Combine(GetWWWRootVirtualPathForHTML(), @"temp");
        }
        
        //From accepted answer at https://stackoverflow.com/questions/5116977/how-to-check-the-os-version-at-runtime-e-g-windows-or-linux-without-using-a-con
        public static bool IsLinux
        {
            get
            {
                int p = (int) Environment.OSVersion.Platform;
                return p == 4 || p == 6 || p == 128;
            }
        }
    }
}
