using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Essential methods for communicating with MySQL Database and Client Browsers with common Date and Time information. 
/// </summary>
namespace WebAPI.Helper_Classes
{
    public static class UnixTimeConverter
    {
        /// <summary>
        /// Creates a DateTime object from a Unix time.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns>Returns a .Net DateTime object from a unix time.</returns>
        public static DateTime toDateTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// Creates a Unix Time long from a DateTime object.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns>Returns a unix time in a long.</returns>
        public static long toUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
    }
}
