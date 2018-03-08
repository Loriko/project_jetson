using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// Represents a response object to an AveragesOfDayRequest from the Front-End client.
    /// Contains an array of 24 PerHourStats, which represents every hour of the day's average number of tracked people.
    /// Serves as a container for many objects in a single object, which is an HTTP limitation.
    /// </summary>
    public class AveragesOfDayResponse
    {
        // Contains hourly averages of an entire day. These will be displayed in a Daily Chart in the Front-End.
        public PerHourStats[] HourlyAverages { get; set; }

        // Constructor
        public AveragesOfDayResponse()
        {
            this.HourlyAverages = new PerHourStats[24];
        }
    }
}
