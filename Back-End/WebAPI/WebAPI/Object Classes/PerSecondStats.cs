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
        public int CameraID { get; set; } 

        // Statistic #1: Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Statistic #2: Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool HasSavedImage { get; set; }

        // Constructor with a flag of false by default for HasSavedImage.
        public PerSecondStats (int cameraID, int year, int month, int day, int hour, int minute, int second, int numTrackedPeople, bool hasSavedImage = false)
        {
            this.CameraID = cameraID;
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.NumTrackedPeople = numTrackedPeople;
            this.HasSavedImage = hasSavedImage;
        }
    }
}
