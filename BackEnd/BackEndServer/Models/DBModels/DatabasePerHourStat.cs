using System;

namespace BackEndServer.Models.DBModels
{
    public class DatabasePerHourStat
    {
        // Table Name
        public static readonly string TABLE_NAME = "perhourstat";
        // Attributes of PerHourStat table.
        public static readonly string HOUR_STAT_ID_LABEL = "id";
        public static readonly string DATE_DAY_LABEL = "date_day";
        public static readonly string DATE_HOUR_LABEL = "date_hour";
        public static readonly string MAX_DETECTED_OBJECT_LABEL = "max_detected_object";
        public static readonly string MIN_DETECTED_OBJECT_LABEL = "min_detected_object";
        public static readonly string AVG_DETECTED_OBJECT_LABEL = "avg_detected_object";

        // Database Model Class Attributes
        public DateTime DateTime { get; set; }
        public double AverageDetectedObjects { get; set; }
        public int MaximumDetectedObjects { get; set; }
        public int MinimumDetectedObjects { get; set; }
    }
}
