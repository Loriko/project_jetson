using BackEndServer.Models.ViewModels;
using Castle.Core.Internal;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseCamera
    {
        // Table Name
        public static readonly string TABLE_NAME = "camera";
        // Attributes of Camera table.
        public static readonly string CAMERA_ID_LABEL = "id";
        public static readonly string CAMERA_NAME_LABEL = "camera_name";
        public static readonly string LOCATION_ID_LABEL = "location_id";
        public static readonly string USER_ID_LABEL = "user_id";
        public static readonly string MONITORED_AREA_LABEL = "monitored_area";
        public static readonly string BRAND_LABEL = "brand";
        public static readonly string MODEL_LABEL = "model";
        public static readonly string RESOLUTION_LABEL = "resolution";

        // Database Model Class Attributes
        public int CameraId { get; set; }
        public string CameraName { get; set; }
        public int LocationId { get; set; }
        public int UserId { get; set; }
        public string MonitoredArea { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Resolution { get; set; }

        public DatabaseCamera()
        {
        }

        public DatabaseCamera(CameraDetails cameraDetails)
        {
            CameraId = cameraDetails.CameraId;
            CameraName = cameraDetails.CameraName;
            LocationId = cameraDetails.LocationId;
            UserId = cameraDetails.UserId;
            MonitoredArea = cameraDetails.MonitoredArea;
            Brand = cameraDetails.Brand;
            Model = cameraDetails.Model;
            Resolution = !cameraDetails.CustomResolution.IsNullOrEmpty() ? cameraDetails.CustomResolution : cameraDetails.Resolution;
        }
    }
}
