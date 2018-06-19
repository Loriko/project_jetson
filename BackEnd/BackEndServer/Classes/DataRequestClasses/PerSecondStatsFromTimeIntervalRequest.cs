using BackEndServer.Classes.EntityDefinitionClasses;
using Newtonsoft.Json;

namespace BackEndServer.Classes.DataRequestClasses
{
    public class PerSecondStatsFromTimeIntervalRequest
    {
        // Attribute(s)
        public string API_Key { get; set; }
        public TimeInterval TimeInterval { get; set; }

        [JsonConstructor]
        public PerSecondStatsFromTimeIntervalRequest(string api_key, TimeInterval timeInterval)
        {
            this.API_Key = api_key;
            this.TimeInterval = timeInterval;
        }
    }
}