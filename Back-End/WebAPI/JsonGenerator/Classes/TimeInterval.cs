using System;

namespace JsonGenerator.Classes
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
    }
}