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
        public int Year { get; }
        public int Month { get; }
        public int Day { get; }

        // Time Attributes. Assuming same timezone between client and API.
        public int Hour { get; }
        public int Minute { get; }
        public int Second { get; }

        // Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool? HasSavedImage { get; }

        // Stores the number of people identified within the second.
        public int NumTrackedPeople { get; }
    }
}
