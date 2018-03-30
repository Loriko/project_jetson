using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Information why uint isn't being used in the API : https://stackoverflow.com/questions/3095805/using-uint-vs-int

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// </summary>
    public class OldPerSecondStat
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
        public OldPerSecondStat (int cameraId, int year, int month, int day, int hour, int minute, int second, int numTrackedPeople, bool hasSavedImage = false)
        {
            CameraId = cameraId;
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            NumTrackedPeople = numTrackedPeople;
            HasSavedImage = hasSavedImage;
        }

        /// <summary>
        /// Checks the validity of the attributes of the PerSecondStat object.
        /// </summary>
        /// <returns>Boolean indicating if the PerSecondStat object is valid.</returns>
        public bool IsValidSecondStat()
        {
            if (CameraId < 0)
                return false;

            if (NumTrackedPeople < 0)
                return false;

            #region Verify Date and Time

            // Start Year
            if (Year < 1900 || Year > 9999)
                return false;

            //Start Month
            if (Month < 1 || Month > 12)
                return false;

            // Validate StartDay based on the month and leap year (for February).
            if (Month == 1 || Month == 3 || Month == 5 || Month == 7 || Month == 8 || Month == 10 || Month == 12)
            {
                if (Day < 1 || Day > 31)
                {
                    return false;    
                }
            }
            else if (Month == 4 || Month == 6 || Month == 9 || Month == 11)
            {
                if (Day < 1 || Day > 30)
                {
                    return false;    
                }
            }
            else
            {
                // Only occurs when: this.StartMonth == 2
                if (DateTime.IsLeapYear(Year))
                {
                    if (Day < 1 || Day > 29)
                    {
                        return false;    
                    }
                }
                else
                {
                    if (Day < 1 || Day > 28)
                    {
                        return false;    
                    }
                }
            }

            // Start Hour
            if (Hour < 0 || Hour > 23)
            {
                return false;    
            }

            // Start Minute
            if (Minute < 0 || Minute > 59)
            {
                return false;    
            }

            // Start Second
            if (Second < 0 || Second > 59)
            {
                return false;    
            }

            #endregion

            return true;
        }
    }
}