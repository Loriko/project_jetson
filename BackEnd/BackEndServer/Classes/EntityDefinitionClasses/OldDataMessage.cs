using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// Object which acts as a container for one or more PerSecondStats objects.
    /// Used to communicate with the Web Server and with the Capture System.
    /// Can carry PerSecondStats objects for an interval of time (ex: 5 seconds) and from multiple cameras (ex: 3) 
    /// which produce one PerSecondStats object each per second of run time.
    /// </summary>
    public class OldDataMessage
    {
        // Attribute(s)
        public OldPerSecondStat[] RealTimeStats { get; set; }

        // Constructor which is also the Json deserialising constructor.
        [JsonConstructor]
        public OldDataMessage(OldPerSecondStat[] PerSecondStats)
        {
            this.RealTimeStats = PerSecondStats;
        }

        // Constructor required by certain specific Web API methods/controllers.
        public OldDataMessage(int NumPerSecondStats)
        {
            this.RealTimeStats = new OldPerSecondStat[NumPerSecondStats];
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
        public bool isValidDataMessage()
        {
            for (int z=0; z < this.getLength(); z++)
            {
                if (!RealTimeStats[z].IsValidSecondStat())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Will be used by the DataReceival controller after a DataMessage is checked not to be valid.
        /// </summary>
        /// <returns>An array of strings indicating the name of the attributes which were detected invalid.</returns>
        public string[] GetInvalidAttributes()
        {
            List<string> temp = new List<string>();

            for (int c = 0; c < this.getLength(); c++)
            {
                if (this.RealTimeStats[c].CameraId < 0)
                    temp.Add("CameraId");

                if (this.RealTimeStats[c].NumTrackedPeople < 0)
                    temp.Add("NumTrackedPeople");

                // TODO: Non unix time equivalent here
//                if (this.RealTimeStats[c].validateDateTime() == false)
//                    temp.Add("UnixTime");
            }

            // Removes all duplicates from the list of failed attributes.
            temp = temp.Distinct().ToList<string>();

            // Counter for next step.
            int x = 0;
            string[] result = new string[temp.Count];

            // Store values in an array of strings.
            foreach (string distinctAttribute in temp)
            {
                result[x] = distinctAttribute;
                x++;
            }

            return (result);
        }
    }
}