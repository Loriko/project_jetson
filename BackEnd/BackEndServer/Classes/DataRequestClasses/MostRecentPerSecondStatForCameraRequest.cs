using Newtonsoft.Json;

namespace BackEndServer.Classes.DataRequestClasses
{
    public class MostRecentPerSecondStatForCameraRequest
    {
        // Attribute(s)
        public string API_Key { get; set; }
        public int CameraId { get; set; }

        [JsonConstructor]
        public MostRecentPerSecondStatForCameraRequest(string api_key, int cameraId)
        {
            this.API_Key = api_key;
            this.CameraId = cameraId;
        }
    }
}