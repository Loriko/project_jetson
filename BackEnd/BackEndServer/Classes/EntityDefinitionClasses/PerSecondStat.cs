using System;
using Newtonsoft.Json;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// An object which represents information extracted for each second of real-time monitoring.
    /// </summary>
    public class PerSecondStat : IComparable<PerSecondStat>
    {
        #region Attributes 

        // Date and Time Information in MySQL Format, representing the exact second represented by the statistics in this object.
        public string DateTime { get; set; }

        // The Key of the camera which produced these statistics for this exact second.
        public string CameraKey { get; set; }

        // Statistic #1: Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Statistic #2: Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool HasSavedImage { get; set; }
        
        // Key frame JPEG image file sent as a string.
        public string FrameAsJpg { get; set; }
        
        #endregion

        public PerSecondStat()
        {
        }

        // Constructor which is also the JSON deserialising constructor.
        [JsonConstructor]
        public PerSecondStat (string dateTime, string cameraKey, int numTrackedPeople, bool hasSavedImage, string frameAsJPG)
        {
            this.DateTime = dateTime;
            this.CameraKey = cameraKey;
            this.NumTrackedPeople = numTrackedPeople;
            this.HasSavedImage = hasSavedImage;
            this.FrameAsJpg = frameAsJPG; // Server responsible for generating a path and saving the path in the DatabasePerSecondStat.
        }

        /// <summary>
        /// Checks the validity of the attributes of the PerSecondStat object.
        /// </summary>
        /// <returns>Boolean indicating if the PerSecondStat object is valid.</returns>
        public bool isValidSecondStat()
        {
            if (NumTrackedPeople < 0 || DateTime.CheckIfSQLFormat() == false)
            {
                return false;
            }

            DateTime toValidate = MySqlDateTimeConverter.ToDateTime(DateTime);

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
            if (CameraKey != other.CameraKey)
            {
                return -1;
            }
            else if (DateTime.Equals(other.DateTime))
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
            return Equals(other);
        }

        protected bool Equals(PerSecondStat other)
        {
            return string.Equals(DateTime, other.DateTime) && string.Equals(CameraKey, other.CameraKey) && NumTrackedPeople == other.NumTrackedPeople;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (DateTime != null ? DateTime.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CameraKey != null ? CameraKey.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ NumTrackedPeople;
                return hashCode;
            }
        }
    }
}
