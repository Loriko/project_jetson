using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// An object which represents the average number of people for every second within a single hour.
    /// Will be calculated and stored in the database upon first request only (when no average has been calculated for it yet),
    /// then will only be queried by the DataRequestController afterwards.
    /// This object represents the average number of people tracked by ALL cameras within the hour.
    /// </summary>
    public class PerHourStats
    {
        // Date Attributes, representing the Date of the statistics. (Primary key is these three attributes.)
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        // Statistic: Average of all the PerSecondStats of ALL CAMERAS for a single hour.
        public double HourlyAverage { get; set; }

        // Constructor with a flag of false by default for HasSavedImage.
        public PerHourStats(int year, int month, int day, double hourlyAverage)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.HourlyAverage = hourlyAverage;
        }
    }
}
