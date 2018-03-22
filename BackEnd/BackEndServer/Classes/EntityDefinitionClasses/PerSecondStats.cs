using System;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// TODO: Go over this. I'm not sure that UnixTime is correct after all since it's not standard
    /// </summary>
    public class PerSecondStats : IComparable<PerSecondStats>
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

        /// <summary>
        /// Checks the validity of the attributes of the PerSecondStat object.
        /// </summary>
        /// <returns>Boolean indicating if the PerSecondStat object is valid.</returns>
        public bool isValidSecondStat()
        {
            if (CameraId < 0)
                return (false);

            if (NumTrackedPeople < 0)
                return (false);

            // Convert Unix Time to DateTime object and verify it.
            DateTime toValidate = this.UnixTime.toDateTime();

            return (DateTimeTools.validateDateTime(toValidate));
        }
        
        /// <summary>
        /// To be used for eliminating possible duplicates in a DataMessage.
        /// Must consider camera ids because a DataMessage can have the same second from multiple cameras.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PerSecondStats other)
        {
            if (this.CameraId != other.CameraId)
                return (-1);

            if (this.UnixTime > other.UnixTime)
                return (-1);
            if (this.UnixTime < other.UnixTime)
                return (1);
            // this.UnixTime == other.UnixTime
            return (0);
        }

        /// <summary>
        /// To be used for eliminating possible duplicates in a DataMessage.
        /// Must consider camera ids because a DataMessage can have the same second from multiple cameras.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PerSecondStats other = obj as PerSecondStats;
            return (other.UnixTime == this.UnixTime && this.CameraId != other.CameraId);
        }
    }
}
