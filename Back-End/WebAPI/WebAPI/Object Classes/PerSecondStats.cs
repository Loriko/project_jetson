using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// </summary>
    public class PerSecondStats
    {
        // The following attributes are defined of passing a DateTime object to the API. This should be simpler for API clients.

        // Date Attributes.
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        // Time Attributes. Assuming same timezone between client and API.
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }

        // Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool? HasSavedImage { get; set; }

        // Constructor with a flag of false by default for HasSavedImage.
        public PerSecondStats (int year, int month, int day, int hour, int minute, int second, int numTrackedPeople, bool hasSavedImage = false)
        {
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
