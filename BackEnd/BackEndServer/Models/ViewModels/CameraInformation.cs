using System;
namespace BackEndServer.Models.ViewModels
{
    public class CameraInformation
    {
        public int CameraId { get; set; }
        public string CameraRoomName { get; set; }
        public string CameraName { get; set; }

        public CameraInformation(int cameraId, string cameraRoomName){
            this.CameraId = cameraId;
            this.CameraRoomName = cameraRoomName;
            this.CameraName = cameraRoomName + " Camera";
        }

        public CameraInformation(int cameraId, string cameraRoomName, string cameraName)
        {
            this.CameraId = cameraId;
            this.CameraRoomName = cameraRoomName;
            this.CameraName = cameraName;
        }
    }
}
