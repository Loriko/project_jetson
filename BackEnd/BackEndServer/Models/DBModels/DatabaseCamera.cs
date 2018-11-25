using BackEndServer.Models.ViewModels;
using Castle.Core.Internal;
using BackEndServer.Models.Enums;
using MySql.Data.MySqlClient;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseCamera
    {
        // Table Name
        public static readonly string TABLE_NAME = "camera";
        // Attributes of Camera table.
        public static readonly string CAMERA_ID_LABEL = "id";
        public static readonly string CAMERA_KEY_LABEL = "camera_key";
        public static readonly string CAMERA_NAME_LABEL = "camera_name";
        public static readonly string LOCATION_ID_LABEL = "location_id";
        public static readonly string USER_ID_LABEL = "user_id";
        public static readonly string ROOM_ID_LABEL = "room_id";
        public static readonly string BRAND_LABEL = "brand";
        public static readonly string MODEL_LABEL = "model";
        public static readonly string RESOLUTION_LABEL = "resolution";
        public static readonly string IMAGE_PATH_LABEL = "image_path";
        // Represents the virtual path that the Images of the Camera's monitored areas will be stored.
        public static readonly string PATH_FOR_USER_UPLOADED_IMAGES = "wwwroot/cameraimages/";

        // Database Model Class Attributes
        public int CameraId { get; set; }
        public string CameraKey { get; set; }
        public string CameraName { get; set; }
        public int? LocationId { get; set; }
        public int? UserId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Resolution { get; set; }
        public string ImagePath { get; set; }
        public int? RoomId { get; set; }

        public DatabaseCamera()
        {
        }

        public DatabaseCamera(CameraDetails cameraDetails)
        {
            CameraId = cameraDetails.CameraId;
            CameraName = cameraDetails.CameraName;
            CameraKey = cameraDetails.CameraKey;
            LocationId = cameraDetails.LocationId;
            UserId = cameraDetails.UserId;
            Brand = cameraDetails.Brand;
            Model = cameraDetails.Model;
            Resolution = !cameraDetails.CustomResolution.IsNullOrEmpty() ? cameraDetails.CustomResolution : cameraDetails.Resolution;
            ImagePath = cameraDetails.SavedImagePath;
            RoomId = cameraDetails.ExistingRoomId;
        }
        
        public void EscapeStringFields()
        {
            if (CameraKey != null)
                CameraKey = MySqlHelper.EscapeString(CameraKey);
            if (CameraName != null)
                CameraName = MySqlHelper.EscapeString(CameraName);
            if (Brand != null)
                Brand = MySqlHelper.EscapeString(Brand);
            if (Model != null)
                Model = MySqlHelper.EscapeString(Model);
            if (Resolution != null)
                Resolution = MySqlHelper.EscapeString(Resolution);
        }
    }
}
