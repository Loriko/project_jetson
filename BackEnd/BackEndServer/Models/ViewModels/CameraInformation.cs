using System;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class CameraInformation
    {
        public int CameraId { get; set; }
        public int RoomId { get; set; }
        public string CameraRoomName { get; set; }
        public string CameraName { get; set; }
        public string ImagePath { get; set; }
        public GraphStatistics GraphStatistics { get; set; }
        public string TempImagePath { get; set; }


        public CameraInformation(int cameraId, string cameraName, string imagePath)
        {
            CameraId = cameraId;
            CameraName = cameraName;
            ImagePath = imagePath;
            TempImagePath = null;
        }

        public CameraInformation(DatabaseCamera dbCamera)
        {
            CameraId = dbCamera.CameraId;
            CameraName = dbCamera.CameraName;
            ImagePath = dbCamera.ImagePath;
            TempImagePath = null;
            RoomId = dbCamera.RoomId.GetValueOrDefault(0);
        }
    }
}
