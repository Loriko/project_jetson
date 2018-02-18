using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// Object which acts as a container for one or more PerSecondStats objects.
    /// </summary>
    public class DataMessage
    {
        public DataMessage(int sizeInSeconds)
        {
            this.MessageSizeSeconds = sizeInSeconds;
            this.RealTimeStats = new PerSecondStats[sizeInSeconds];
        }

        public PerSecondStats[] RealTimeStats { get; set; }
        public int? MessageSizeSeconds { get; set; }
    }
}
