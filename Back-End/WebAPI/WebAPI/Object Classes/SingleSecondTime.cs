using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helper_Classes;

namespace WebAPI.Object_Classes
{
    public class SingleSecondTime
    {
        public long UnixTime { get; set; }

        public SingleSecondTime(long UnixTime)
        {
            this.UnixTime = UnixTime;
        }

        public bool isValidSingleSecondTime()
        {
            // Convert from Unix Time to a Date Time object.
            DateTime time = this.UnixTime.toDateTime();

            // Validate Date Time object and return validation result (valid or invalid).
            return (DateTimeTools.validateDateTime(time)); 
        }
    }
}