using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// Represents a DateTime interval. Simplified without time zones.
    /// </summary>
    public class TimeInterval
    {
        // Start DateTime.
        public int StartYear { get; }
        public int StartMonth { get; }
        public int StartDay { get; }
        public int StartHour { get; }
        public int StartMinute { get; }
        public int StartSecond { get; }

        // End DateTime.
        public int EndYear { get; }
        public int EndMonth { get; }
        public int EndDay { get; }
        public int EndHour { get; }
        public int EndMinute { get; }
        public int EndSecond { get; }
    }
}
