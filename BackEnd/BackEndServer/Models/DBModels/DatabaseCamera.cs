using BackEndServer.Models.ViewModels;
using Castle.Core.Internal;
using BackEndServer.Models.Enums;

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
        public static readonly string MONITORED_AREA_LABEL = "monitored_area";
        public static readonly string BRAND_LABEL = "brand";
        public static readonly string MODEL_LABEL = "model";
        public static readonly string RESOLUTION_LABEL = "resolution";
        public static readonly string IMAGE_PATH_LABEL = "image_path";
        // Represents the virtual path that the Images of the Camera's monitored areas will be stored.
        public static readonly string PATH_FOR_USER_UPLOADED_IMAGES = "";
        public static readonly string JPEG_EXTENSION = ".jpg";
        public static readonly string PNG_EXTENSION = ".png";

        // Database Model Class Attributes
        public int CameraId { get; set; }
        public string CameraKey { get; set; }
        public string CameraName { get; set; }
        public int? LocationId { get; set; }
        public int? UserId { get; set; }
        public string MonitoredArea { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Resolution { get; set; }
        public string ImagePath { get; set; }

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
            MonitoredArea = cameraDetails.MonitoredArea;
            Brand = cameraDetails.Brand;
            Model = cameraDetails.Model;
            Resolution = !cameraDetails.CustomResolution.IsNullOrEmpty() ? cameraDetails.CustomResolution : cameraDetails.Resolution;

            if (cameraDetails.HasUploadedImage && cameraDetails.UploadedImageFileType == (int)ImageFileType.JPEG)
            {
                ImagePath = PATH_FOR_USER_UPLOADED_IMAGES + CameraId + JPEG_EXTENSION;
            }
            else if (cameraDetails.HasUploadedImage && cameraDetails.UploadedImageFileType == (int)ImageFileType.PNG)
            {
                ImagePath = PATH_FOR_USER_UPLOADED_IMAGES + CameraId + PNG_EXTENSION;
            }
        }
    }
}
