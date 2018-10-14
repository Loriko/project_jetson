using System;

namespace ConsoleApp1
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// TODO: Go over this. I'm not sure that UnixTime is correct after all since it's not standard
    /// </summary>
    public class PerSecondStat
    {
        // Date and Time Information in MySQL Format, representing the exact second represented by the statistics in this object.
        public string DateTime { get; set; }

        // The Key of the camera which produced these statistics for this exact second.
        public string CameraKey { get; set; }

        // Statistic #1: Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Statistic #2: Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool HasSavedImage { get; set; }

        public PerSecondStat(string dateTime, string cameraKey, int numTrackedPeople, bool hasSavedImage)
        {
            this.DateTime = dateTime;
            this.CameraKey = cameraKey;
            this.NumTrackedPeople = numTrackedPeople;
            this.HasSavedImage = hasSavedImage;
        }
    }
}
