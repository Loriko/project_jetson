using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndServer.Services.HelperServices
{
    // My SQL DateTime format: '9999-12-31 23:59:59'

    public static class MySqlDateTimeConverter
    {
        public static bool CheckIfSQLFormat(this string dateTimeString)
        {
            // TO IMPLEMENT
            throw new NotImplementedException();
        }

        public static string toMySqlDateTime(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime toDateTime(this string mySqlStringDate)
        {
            DateTime result = DateTime.Parse(mySqlStringDate);
            return (result);
        }
    }
}
