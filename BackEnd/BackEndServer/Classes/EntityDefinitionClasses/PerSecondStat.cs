using System;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// TODO: Go over this. I'm not sure that UnixTime is correct after all since it's not standard
    /// </summary>
    public class PerSecondStat : IComparable<PerSecondStat>
    {
        #region Attributes 

        // Date and Time Information in MySQL Format, representing the exact second represented by the statistics in this object.
        public string DateTime { get; set; }

        // The ID of the camera which produced these statistics for this exact second.
        public int CameraId { get; set; } 

        // Statistic #1: Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Statistic #2: Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool HasSavedImage { get; set; }

        #endregion

        public PerSecondStat (string dateTime, int cameraId, int numTrackedPeople, bool hasSavedImage)
        {
            this.DateTime = dateTime;
            this.CameraId = cameraId;
            this.NumTrackedPeople = numTrackedPeople;
            this.HasSavedImage = hasSavedImage;
        }

        /// <summary>
        /// Checks the validity of the attributes of the PerSecondStat object.
        /// </summary>
        /// <returns>Boolean indicating if the PerSecondStat object is valid.</returns>
        public bool isValidSecondStat()
        {
            if (this.CameraId < 0 || this.NumTrackedPeople < 0 || this.DateTime.CheckIfSQLFormat() == false)
            {
                return false;
            }

            DateTime toValidate = MySqlDateTimeConverter.ToDateTime(this.DateTime);

            return DateTimeTools.ValidateDateTime(toValidate);
        }
        
        /// <summary>
        /// To be used for eliminating possible duplicates in a DataMessage.
        /// Must consider camera ids because a DataMessage can have the same second from multiple cameras.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PerSecondStat other)
        {
            if (this.CameraId != other.CameraId)
            {
                return -1;
            }
            else if (this.DateTime.Equals(other.DateTime))
            {
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// To be used for eliminating possible duplicates in a DataMessage.
        /// Must consider camera ids because a DataMessage can have the same second from multiple cameras.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PerSecondStat other = obj as PerSecondStat;
            return (other.DateTime == this.DateTime && this.CameraId == other.CameraId);
        }
    }
}
