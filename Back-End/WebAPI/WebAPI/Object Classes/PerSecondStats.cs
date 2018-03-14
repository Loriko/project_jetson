using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Helper_Classes;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// </summary>
    public class PerSecondStats
    {
        #region Attributes 

        // Date and Time Information, representing the exact second represented by the statistics in this object.
        public long UnixTime { get; set; }

        // The ID of the camera which produced these statistics for this exact second.
        public int CameraId { get; set; } 

        // Statistic #1: Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Statistic #2: Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool HasSavedImage { get; set; }

        #endregion

        // Constructor with a flag of false by default for HasSavedImage.
        public PerSecondStats (int cameraId, long unixTime, int numTrackedPeople, bool hasSavedImage = false)
        {
            this.CameraId = cameraId;
            this.UnixTime = unixTime;
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

            // Convert to DateTime object and verify it.

            DateTime toCheck = this.UnixTime.toDateTime();
            int Year = toCheck.Year;
            int Month = toCheck.Month;
            int Day = toCheck.Day;
            int Hour = toCheck.Hour;
            int Minute = toCheck.Minute;
            int Second = toCheck.Second;

            #region Verify Date and Time

            // Start Year
            if (Year < 1900 || Year > 9999)
                return (false);

            //Start Month
            if (Month < 1 || Month > 12)
                return (false);

            // Validate StartDay based on the month and leap year (for February).
            if (Month == 1 || Month == 3 || Month == 5 || Month == 7 || Month == 8 || Month == 10 || Month == 12)
            {
                if (Day < 1 || Day > 31)
                    return (false);
            }
            else if (Month == 4 || Month == 6 || Month == 9 || Month == 11)
            {
                if (Day < 1 || Day > 30)
                    return (false);
            }
            else
            {
                // Only occurs when: StartMonth == 2
                if (DateTime.IsLeapYear((int)Year))
                {
                    if (Day < 1 || Day > 29)
                        return (false);
                }
                else
                {
                    if (Day < 1 || Day > 28)
                        return (false);
                }
            }

            // Start Hour
            if (Hour < 1 || Hour > 23)
                return (false);

            // Start Minute
            if (Minute < 0 || Minute > 59)
                return (false);

            // Start Second
            if (Second < 0 || Second > 59)
                return (false);

            #endregion

            return (true);
        }
    }
}
