using System;

namespace BackEndServer.Models.DBModels
{
    public class DatabasePerSecondStat
    {
        public int StatId { get; set; }
        public string DateTime { get; set; }
        public bool HasSavedImage { get; set; }
        public int NumDetectedObjects { get; set; }
        public int CameraId { get; set; }
    }
}