using Newtonsoft.Json;

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// Object which acts as a container for one or more PerSecondStat objects.
    /// Used to communicate with the Web Server and with the Capture System.
    /// Can carry PerSecondStat objects for an interval of time (ex: 5 seconds) and from multiple cameras (ex: 3) 
    /// which produce one PerSecondStat object each per second of run time.
    /// </summary>
    public class DataMessage
    {
        // Attribute(s)
        public string API_Key { get; set; }
        public PerSecondStat[] RealTimeStats { get; set; }

        // Constructor which is also the JSON deserialising constructor.
        [JsonConstructor]
        public DataMessage(string api_key, PerSecondStat[] perSecondStats)
        {
            this.API_Key = api_key;
            this.RealTimeStats = perSecondStats;
        }

        /// <summary>
        /// Returns the number of PerSecondStat objects stored within the DataMessage. A.K.A. the size of the RealTimeStats attribute array.
        /// </summary>
        /// <returns>Number of PerSecondStat objects within the DataMessage.</returns>
        public int GetLength()
        {
            return this.RealTimeStats.Length;
        }

        /// <summary>
        /// Checks if the DataMessage is empty.
        /// </summary>
        /// <returns>TRUE if DataMessage is empty, false otherwise.</returns>
        public bool IsEmpty()
        {
            // Check if null before numeric comparision to avoid exceptions.
            if (this.RealTimeStats == null)
            {
                return true;
            }
            else if (this.GetLength() < 1)
            {
                return true;
            }

            return false;
        }
    }
}