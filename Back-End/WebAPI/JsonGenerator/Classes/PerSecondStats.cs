using System;

namespace JsonGenerator.Classes
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// </summary>
    public class PerSecondStats
    {
        #region Attributes 

        // Date and Time Information, representing the exact second represented by the statistics in this object.
        public long UnixTime { get; set; }

        // The ID of the camera which produced these statistics for this exact second.
        public int CameraId { get; set; } 

        // Statistic #1: Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Statistic #2: Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool HasSavedImage { get; set; }

        #endregion

        // Constructor with a flag of false by default for HasSavedImage.
        public PerSecondStats (int CameraId, long UnixTime, int NumTrackedPeople, bool HasSavedImage = false)
        {
            this.CameraId = CameraId;
            this.UnixTime = UnixTime;
            this.NumTrackedPeople = NumTrackedPeople;
            this.HasSavedImage = HasSavedImage;
        }
    }
}
