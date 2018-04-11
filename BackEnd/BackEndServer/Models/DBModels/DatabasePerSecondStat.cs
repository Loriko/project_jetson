using System;

namespace BackEndServer.Models.DBModels
{
    public class DatabasePerSecondStat
    {
        public static readonly string TABLE_NAME = "perSecondStat";
        public static readonly string STAT_ID_LABEL = "idStat";
        public static readonly string CAMERA_ID_LABEL = "Camera_idCamera";
        public static readonly string DATE_TIME_LABEL = "dateTime";
        public static readonly string HAS_SAVED_IMAGE_LABEL = "hasSavedImage";
        public static readonly string NUM_DETECTED_OBJECTS_LABEL = "numDetectedObjects";
        
        public int StatId { get; set; }
        public DateTime DateTime { get; set; }
        public bool HasSavedImage { get; set; }
        public int NumDetectedObjects { get; set; }
        public int CameraId { get; set; }
    }
}