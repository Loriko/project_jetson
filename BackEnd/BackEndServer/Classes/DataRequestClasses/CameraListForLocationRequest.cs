using Newtonsoft.Json;

namespace BackEndServer.Classes.DataRequestClasses
{
    public class CameraListForLocationRequest
    {
        // Attribute(s)
        public string API_Key { get; set; }
        public int LocationId { get; set; }

        [JsonConstructor]
        public CameraListForLocationRequest(string api_key, int locationId)
        {
            this.API_Key = api_key;
            this.LocationId = locationId;
        }
    }
}