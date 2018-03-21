
namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// An object which represents the average number of people for every second within a single hour.
    /// Will be calculated and stored in the database upon first request only (when no average has been calculated for it yet),
    /// then will only be queried by the DataRequestController afterwards.
    /// This object represents the average number of people tracked by ALL cameras within the hour.
    /// </summary>
    public class PerHourStats
    {
        // Ignoring minutes and seconds. Returned in responses. Is in a format easily supported by JavaScript.
        public long UnixTime { get; set; }
        // Statistic: Average of all the PerSecondStats of ALL CAMERAS for a single hour.
        public double HourlyAverage { get; set; }

        public PerHourStats(long UnixTime, double HourlyAverage)
        {
            this.UnixTime = UnixTime;
            this.HourlyAverage = HourlyAverage;
        }

        // No validation needed as this is never provided by browser client requests, only sent in responses to web server.
    }
}
