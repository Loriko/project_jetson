using System;
using System.IO;

namespace BackEndServer.Services.HelperServices
{
    public static class RootDirectoryTools
    {
        public static string GetWWWRootPhysicalPath()
        {
            return Path.Combine
            (
                // "<PROJECT_ROOT>\\BackEnd\\BackEndServer\\bin\\Debug\\netcoreapp2.0\\"
                AppDomain.CurrentDomain.BaseDirectory, 
                // Go up 3 parent levels.
                @"..\..\..\",
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
    }
}
