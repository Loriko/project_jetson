using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class CameraDetails
    {
        public int CameraId { get; set; }
        public string CameraName { get; set; }
        public int LocationId { get; set; }
        public int UserId { get; set; }
        public string MonitoredArea { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Resolution { get; set; }
        public string CustomResolution { get; set; }
        
        public CameraDetails()
        {
            //TODO maybe set UserId to current's user id
        }

        public CameraDetails(DatabaseCamera dbCamera)
        {
            CameraId = dbCamera.CameraId;
            CameraName = dbCamera.CameraName;
            LocationId = dbCamera.LocationId;
            UserId = dbCamera.UserId;
            MonitoredArea = dbCamera.MonitoredArea;
            Brand = dbCamera.Brand;
            Model = dbCamera.Model;
            Resolution = dbCamera.Resolution;
        }
    }
}