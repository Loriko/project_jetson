using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// Object which acts as a container for one or more PerSecondStats objects.
    /// Used to communicate with the Web Server and with the Capture System.
    /// Can carry PerSecondStats objects for an interval of time (ex: 5 seconds) and from multiple cameras (ex: 3) 
    /// which produce one PerSecondStats object each per second of run time.
    /// </summary>
    public class DataMessage
    {
        // Attribute
        public PerSecondStats[] RealTimeStats { get; set; }

        [JsonConstructor]
        public DataMessage(PerSecondStats[] perSecondStats)
        {
            this.RealTimeStats = perSecondStats;
        }

        public DataMessage(int numPerSecondStats)
        {
            this.RealTimeStats = new PerSecondStats[numPerSecondStats];
        }

        /// <summary>
        /// Returns the number of PerSecondStats objects stored within the DataMessage. A.k.a the size of the RealTimeStats attribute array.
        /// </summary>
        /// <returns>Number of PerSecondStats objects within</returns>
        public int getLength()
        {
            return (this.RealTimeStats.Length);
        }

        /// <summary>
        /// Verifies validity of all contents of the DataMessage.
        /// </summary>
        /// <returns>Boolean indicating if DataMessage is valid.</returns>
        public bool isValidMessage()
        {
            for (int z=0; z < this.getLength(); z++)
            {
                if (this.RealTimeStats[z].isValidSecondStat() == false)
                    return (false);
            }
            return (true);
        }
    }
}