using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class DatabaseCamera
    {
        private StatisticsDatabaseContext databaseContext;
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
