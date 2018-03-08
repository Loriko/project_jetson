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
        public PerSecondStats[] RealTimeStats { get; set; }

        // Completely optional parameter for clients to provide, however when API returns this type of object, attribute will be filled up for simplicity for clients. 
        public int? MessageSizeSeconds { get; set; }

        // Constructor
        public DataMessage(int sizeInSeconds)
        {
            this.MessageSizeSeconds = sizeInSeconds;
            this.RealTimeStats = new PerSecondStats[sizeInSeconds];
        }
    }
}
