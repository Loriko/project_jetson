using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackEndServer.Services.HelperServices
{
    // My SQL DateTime format: '9999-12-31 23:59:59'

    public static class MySqlDateTimeConverter
    {
        // [API_UnitTest_1] - PASS (Mohamed R.)
        public static bool CheckIfSQLFormat(this string dateTimeString)
        {
            Regex regex = new Regex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$");
            return regex.IsMatch(dateTimeString);
        }

        // [API_UnitTest_2] - PASS (Mohamed R.)
        public static string toMySqlDateTime(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss"); 
        }

        // [API_UnitTest_3] - PASS (Mohamed R.)
        public static DateTime toDateTime(this string mySqlStringDate)
        {
            DateTime result = DateTime.Parse(mySqlStringDate);
            return (result);
        }
    }
}
