using System;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseGraphStat
    {
        // Table Name
        public static readonly string TABLE_NAME = "per_second_stat";
        // Attributes of PerSecondStat table.
        public static readonly string PER_SECOND_STAT_ID_LABEL = "id";
        public static readonly string CAMERA_ID_LABEL = "camera_id";
        public static readonly string NUM_DETECTED_OBJECTS_LABEL = "num_detected_object";
        public static readonly string DATE_TIME_LABEL = "date_time";
        public static readonly string HAS_SAVED_IMAGE_LABEL = "has_saved_image";
        public static readonly string PER_HOUR_STAT_ID_LABEL = "per_hour_stat_id";

        // Database Model Class Attributes
        public string CameraID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double AverageDetectedObjects { get; set; }
        public int MaximumDetectedObjects { get; set; }
        public int MinimumDetectedObjects { get; set; }
    }
}