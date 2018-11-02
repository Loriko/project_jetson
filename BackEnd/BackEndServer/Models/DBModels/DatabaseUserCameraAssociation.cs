using BackEndServer.Models.ViewModels;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseUserCameraAssociation
    {
        // Table Name
        public static readonly string TABLE_NAME = "user_camera_association";
        // Attributes of U.
        public static readonly string USER_ID = "user_id";
        public static readonly string CAMERA_ID = "camera_id";
        
        public int CameraId { get; set;}
        public int UserId { get; set; }

        public DatabaseUserCameraAssociation()
        {
        }
        
        public DatabaseUserCameraAssociation(UserCameraAssociation userCameraAssociation)
        {
            CameraId = userCameraAssociation.CameraId;
            UserId = userCameraAssociation.UserId;
        }
    }
}