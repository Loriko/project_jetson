using System;
using System.Text.RegularExpressions;

namespace BackEndServer.Services.HelperServices
{
    // My SQL DateTime format: '9999-12-31 23:59:59'

    public static class MySqlDateTimeConverter
    {
        // Does not validate validity of the datetime, only the format.
        public static bool CheckIfSQLFormat(this string dateTimeString)
        {
            Regex regex = new Regex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}( [+-][0-9]{4})?$");
            return regex.IsMatch(dateTimeString);
        }

        public static string ToMySqlDateTimeString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss"); 
        }

        public static string ToMySqlDateString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        public static DateTime ToDateTime(this string mySqlStringDate)
        {
            DateTime dateValue;
            bool success = DateTime.TryParse(mySqlStringDate, out dateValue);

            if (!success)
            {
                // TODO Log error here
                throw new FormatException("Invalid SQL DateTime Format When Parsing.");
            }

            return (dateValue);
        }
    }
}
