using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// Represents a request for the hourly averages of an entire day. Simplified without time zones.
    /// </summary>
    public class AveragesOfDayRequest
    {
        #region Attributes
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        #endregion 

        /*
        [JsonConstructor]
        public AveragesOfDayRequest(int Year, int Month, int Day)
        {
            this.Year = Year;
            this.Month = Month;
            this.Day = Day;
        }
        */

        // Only the Front-End clients would create this object. API will use the hasValidDay validation method before processing request.
        public AveragesOfDayRequest() { }

        /// <summary>
        /// Method to validate a AveragesOfDayRequest object by checking if the requested day is valid.
        /// </summary>
        /// <returns>Returns TRUE if the date is valid, returns FALSE if date is not valid.</returns>
        public bool hasValidDay()
        {
            #region Verify Start Attributes (lower bound of interval)
            // Start Year
            if (this.Year < 1900 || this.Year > 9999)
                return (false);

            //Start Month
            if (this.Month < 1 || this.Month > 12)
                return (false);

            // Validate StartDay based on the month and leap year (for February).
            if (this.Month == 1 || this.Month == 3 || this.Month == 5 || this.Month == 7 || this.Month == 8 || this.Month == 10 || this.Month == 12)
            {
                if (this.Day < 1 || this.Day > 31)
                    return (false);
            }
            else if (this.Month == 4 || this.Month == 6 || this.Month == 9 || this.Month == 11)
            {
                if (this.Day < 1 || this.Day > 30)
                    return (false);
            }
            else
            {
                // Only occurs when: this.StartMonth == 2
                if (DateTime.IsLeapYear((int)this.Year))
                {
                    if (this.Day < 1 || this.Day > 29)
                        return (false);
                }
                else
                {
                    if (this.Day < 1 || this.Day > 28)
                        return (false);
                }
            }
            #endregion

            return (true);
        }
    }
}