using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Information why uint isn't being used in the API : https://stackoverflow.com/questions/3095805/using-uint-vs-int

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// </summary>
    public class PerSecondStats
    {
        // The following attributes are defined of passing a DateTime object to the API. This should be simpler for API clients.

        // Date Attributes, representing the Date of the statistics.
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        // Time Attributes, representing the exact time, to second precision, of the statistics. Assuming same timezone between client and API for initial simplicity.
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }

        // The ID of the camera which produced these statistics for this exact second.
        public int CameraId { get; set; } 

        // Statistic #1: Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Statistic #2: Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool HasSavedImage { get; set; }

        // Constructor with a flag of false by default for HasSavedImage.
        public PerSecondStats (int cameraId, int year, int month, int day, int hour, int minute, int second, int numTrackedPeople, bool hasSavedImage = false)
        {
            this.CameraId = cameraId;
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.NumTrackedPeople = numTrackedPeople;
            this.HasSavedImage = hasSavedImage;
        }

        /// <summary>
        /// Checks the validity of the attributes of the PerSecondStat object.
        /// </summary>
        /// <returns>Boolean indicating if the PerSecondStat object is valid.</returns>
        public bool isValidSecondStat()
        {
            if (CameraId < 0)
                return (false);

            if (NumTrackedPeople < 0)
                return (false);

            #region Verify Date and Time

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

            // Start Hour
            if (this.Hour < 1 || this.Hour > 23)
                return (false);

            // Start Minute
            if (this.Minute < 0 || this.Minute > 59)
                return (false);

            // Start Second
            if (this.Second < 0 || this.Second > 59)
                return (false);

            #endregion

            return (true);
        }
    }
}
