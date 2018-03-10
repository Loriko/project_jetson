using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Newtonsoft.Json;

namespace WebAPI.Object_Classes
{
    /// <summary>
    /// Object which acts as a container for one or more PerSecondStats objects.
    /// Can carry PerSecondStats objects for an interval of time (ex: 5 seconds) and from multiple cameras (ex: 3) 
    /// which produce one PerSecondStats object each per second of run time.
    /// </summary>
    public class DataMessage
    {
        #region Attributes
        public PerSecondStats[] RealTimeStats { get; set; }
        #endregion

        #region Constructors and Json Deserializing Constructors
        /*
        [JsonConstructor]
        public DataMessage(PerSecondStats[] RealTimeStats)
        {
            this.RealTimeStats = RealTimeStats;
        }
        */

        /// <summary>
        /// Normal Constructor
        /// </summary>
        /// <param name="sizeInSeconds">The number of PerSecondStats objects in the DataMessage. If DataMessage holds 5 seconds of data from 2 cameras, in example, then this size should be 10 (PerSecondStats objects).</param>
        public DataMessage(int sizeInSeconds)
        {
            this.RealTimeStats = new PerSecondStats[sizeInSeconds];
        }
        #endregion

        /// <summary>
        /// Returns the number of PerSecondStats objects stored within the DataMessage. A.k.a the size of the RealTimeStats attribute array.
        /// </summary>
        /// <returns>Number of PerSecondStats objects within</returns>
        public int getNumberSeconds()
        {
            return (this.RealTimeStats.Length);
        }
    }
}