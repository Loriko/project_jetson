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
        [JsonConstructor]
        public DataMessage(string cameraId, PerSecondStats[] perSecondStats)
        {
            this.CameraId = cameraId;
            this.RealTimeStats = perSecondStats;
        }

        public string CameraId { get; set; }
        public PerSecondStats[] RealTimeStats { get; set; }
        
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
