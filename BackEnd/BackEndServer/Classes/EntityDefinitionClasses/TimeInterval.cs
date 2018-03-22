using System;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// Represents a DateTime interval using Unix Time attributes. Received during/from Web Server requests.
    /// </summary>
    public class TimeInterval
    {
        public long StartUnixTime { get; set; }
        public long EndUnixTime { get; set; }

        // Constructor required only for ASP.Net Core's automatic Json deserialization.
        // Web API will not need to create TimeInterval objects.
        public TimeInterval(long StartUnixTime, long EndUnixTime)
        {
            this.StartUnixTime = StartUnixTime;
            this.EndUnixTime = EndUnixTime;
        }

        public bool isValidTimeInterval()
        {
            DateTime startTime = this.StartUnixTime.toDateTime();
            DateTime endTime = this.EndUnixTime.toDateTime();

            if (DateTimeTools.validateDateTime(startTime) == false)
                return (false);

            if (DateTimeTools.validateDateTime(endTime) == false)
                return (false);

            // The start time must be before the end time or the start time must be identical to the end time.
            if (DateTime.Compare(startTime, endTime) > 0)
                return (false);

            return (true);
        }
    }
}