namespace BackEndServer.Classes.EntityDefinitionClasses
{
    public class Camera
    {
        public int CameraId { get; set; }
        public string CameraName { get; set; }
        public string MonitoredArea { get; set; }
        public string CameraLocation { get; set; }
        public int LocationId { get; set; }
        public int UserId { get; set; }
        public string CameraBrand { get; set; }
        public string CameraModel { get; set; }
        public string CameraResolution { get; set; }

        public Camera (int CameraId, string CameraName, string MonitoredArea, string CameraLocation, int LocationId, int UserId, string CameraBrand = "", string CameraModel = "", string CameraResolution = "")
        {
            this.CameraId = CameraId;
            this.CameraName = CameraName;
            this.MonitoredArea = MonitoredArea;
            this.CameraLocation = CameraLocation;
            this.LocationId = LocationId;
            this.UserId = UserId;
            this.CameraBrand = CameraBrand;
            this.CameraModel = CameraModel;
            this.CameraResolution = CameraResolution;
        }
    }
}
