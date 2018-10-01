using System;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class CameraInformation
    {
        public int CameraId { get; set; }
        public string CameraRoomName { get; set; }
        public string CameraName { get; set; }
        public string ImagePath { get; set; }
        public GraphStatistics GraphStatistics { get; set; }
        public string TempImagePath { get; set; }


        public CameraInformation(int cameraId, string cameraRoomName, string imagePath){
            CameraId = cameraId;
            CameraRoomName = cameraRoomName;
            CameraName = cameraRoomName + " Camera";
            ImagePath = imagePath;
            TempImagePath = null;
        }

        public CameraInformation(int cameraId, string cameraRoomName, string cameraName, string imagePath)
        {
            CameraId = cameraId;
            CameraRoomName = cameraRoomName;
            CameraName = cameraName;
            ImagePath = imagePath;
            TempImagePath = null;
        }

        public CameraInformation(DatabaseCamera dbCamera) : this(dbCamera.CameraId, dbCamera.MonitoredArea, dbCamera.CameraName, dbCamera.ImagePath) {}
    }
}
