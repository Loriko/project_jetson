using System;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class CameraInformation
    {
        public int CameraId { get; set; }
        public string CameraRoomName { get; set; }
        public string CameraName { get; set; }
        public GraphStatistics GraphStatistics { get; set; }

        public CameraInformation(int cameraId, string cameraRoomName){
            CameraId = cameraId;
            CameraRoomName = cameraRoomName;
            CameraName = cameraRoomName + " Camera";
        }

        public CameraInformation(int cameraId, string cameraRoomName, string cameraName)
        {
            CameraId = cameraId;
            CameraRoomName = cameraRoomName;
            CameraName = cameraName;
        }

        public CameraInformation(DatabaseCamera dbCamera) : this(dbCamera.CameraId, dbCamera.MonitoredArea, dbCamera.CameraName) {}
    }
}
