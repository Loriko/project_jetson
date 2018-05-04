using System;

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
        public static readonly string RESOLUTION_WIDTH_LABEL = "resolution_width";
        public static readonly string RESOLUTION_HEIGHT_LABEL = "resolution_height";

        // Database Model Class Attributes
        public int CameraId { get; set; }
        public string CameraName { get; set; }
        public int LocationId { get; set; }
        public int UserId { get; set; }
        public string MonitoredArea { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int ResolutionWidth { get; set; }
        public int ResolutionHeight { get; set; }
    }
}
