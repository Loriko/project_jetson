using System;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class CameraInformation
    {
        public int CameraId { get; set; }
        public string CameraRoomName { get; set; }
        public string CameraName { get; set; }
        public int CameraUserId { get; set; }
        public GraphStatistics GraphStatistics { get; set; }

<<<<<<< HEAD
        public CameraInformation(int cameraId, string cameraRoomName, int cameraUserId){
            this.CameraId = cameraId;
            this.CameraRoomName = cameraRoomName;
            this.CameraName = cameraRoomName + " Camera";
            this.CameraUserId = cameraUserId;
=======
        public CameraInformation(int cameraId, string cameraRoomName){
            CameraId = cameraId;
            CameraRoomName = cameraRoomName;
            CameraName = cameraRoomName + " Camera";
>>>>>>> master
        }

        public CameraInformation(int cameraId, string cameraRoomName, string cameraName, int cameraUserId)
        {
<<<<<<< HEAD
            this.CameraId = cameraId;
            this.CameraRoomName = cameraRoomName;
            this.CameraName = cameraName;
            this.CameraUserId = cameraUserId;
=======
            CameraId = cameraId;
            CameraRoomName = cameraRoomName;
            CameraName = cameraName;
>>>>>>> master
        }

        public CameraInformation(DatabaseCamera dbCamera) : this(dbCamera.CameraId, dbCamera.MonitoredArea, dbCamera.CameraName, dbCamera.UserId) {}
    }
}
