using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Services;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseCamera
    {
        private DatabaseQueryService _databaseQueryService;
        public int CameraId { get; set; }
        public string CameraName { get; set; }
        public string MonitoredArea { get; set; }
        public string CameraLocation { get; set; }
        public int LocationId { get; set; }
        public int UserID { get; set; }
        public string CameraBrand { get; set; }
        public string CameraModel { get; set; }
        public string CameraResolution { get; set; }
    }
}
