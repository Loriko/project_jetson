using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JsonGenerator.Classes
{
    /// <summary>
    /// Object which acts as a container for one or more PerSecondStats objects.
    /// Used to communicate with the Web Server and with the Capture System.
    /// Can carry PerSecondStats objects for an interval of time (ex: 5 seconds) and from multiple cameras (ex: 3) 
    /// which produce one PerSecondStats object each per second of run time.
    /// </summary>
    public class DataMessage
    {
        // Attribute(s)
        public PerSecondStats[] RealTimeStats { get; set; }

        // Constructor which is also the Json deserialising constructor.
        [JsonConstructor]
        public DataMessage(PerSecondStats[] PerSecondStats)
        {
            this.RealTimeStats = PerSecondStats;
        }

        // Constructor required by certain specific Web API methods/controllers.
        public DataMessage(int NumPerSecondStats)
        {
            this.RealTimeStats = new PerSecondStats[NumPerSecondStats];
        }
    }
}