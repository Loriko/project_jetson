using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndServer.Services.HelperServices
{
    // My SQL DateTime format: '9999-12-31 23:59:59'

    public static class MySqlDateTimeConverter
    {
        public static string toMySqlDateTime(this DateTime time)
        {
            string year = time.Year.ToString();
            string month = time.Month.ToString();
            string day = time.Day.ToString();
            string hour = time.Hour.ToString();
            string minute = time.Minute.ToString();
            string second = time.Second.ToString();

            string result = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;
            return (result);
        }

        public static DateTime toDateTime(this string mySqlStringDate)
        {
            DateTime result = DateTime.ParseExact(mySqlStringDate, "yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture);
            return (result);
        }
    }
}
