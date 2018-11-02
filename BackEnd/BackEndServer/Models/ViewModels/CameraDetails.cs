using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.Enums;
using System;
using Microsoft.AspNetCore.Http;

namespace BackEndServer.Models.ViewModels
{
    public class CameraDetails
    {
        public int CameraId { get; set; }
        public string CameraKey { get; set; }
        public string CameraName { get; set; }
        public int LocationId { get; set; }
        public int UserId { get; set; }
        public string MonitoredArea { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Resolution { get; set; }
        public string CustomResolution { get; set; }
        public LocationDetails Location { get; set; }
        // Supports users uploading a picture of what an installed camera is tracking (the monitored area).
        public IFormFile UploadedImage { get; set; }
        public string SavedImagePath { get; set; }
        public bool ImageDeleted { get; set; }
        public int ExistingRoomId { get; set; }
        public string NewRoomName { get; set; }

        public CameraDetails()
        {
            //TODO maybe set UserId to current's user id
        }

        public CameraDetails(DatabaseCamera dbCamera)
        {
            CameraId = dbCamera.CameraId;
            CameraKey = dbCamera.CameraKey;
            CameraName = dbCamera.CameraName;
            LocationId = dbCamera.LocationId.GetValueOrDefault(0);
            UserId = dbCamera.UserId.GetValueOrDefault(0);
            Brand = dbCamera.Brand;
            Model = dbCamera.Model;
            Resolution = dbCamera.Resolution;
            SavedImagePath = dbCamera.ImagePath;
            ExistingRoomId = dbCamera.RoomId.GetValueOrDefault(0);
        }
    }
}