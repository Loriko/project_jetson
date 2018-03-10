using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// Object which acts as a container for one or more PerSecondStats objects.
    /// </summary>
    public class DataMessage
    {
        public PerSecondStats[] RealTimeStats { get; set; }

        [JsonConstructor]
        public DataMessage(PerSecondStats[] perSecondStats)
        {
            this.RealTimeStats = perSecondStats;
        }

        // Normal constructor.
        public DataMessage(int sizeInSeconds)
        {
            this.RealTimeStats = new PerSecondStats[sizeInSeconds];
        }
        
        /// <summary>
        /// Returns the number of PerSecondStats objects stored within the DataMessage. A.k.a the size of the RealTimeStats attribute array.
        /// </summary>
        /// <returns></returns>
        public int getNumberSeconds()
        {
            return (this.RealTimeStats.Length);
        }
    }
}