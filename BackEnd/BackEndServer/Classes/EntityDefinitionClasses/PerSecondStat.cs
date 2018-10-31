using System;
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

        // The Database Id of the camera which produced these statistics for this exact second
        //TODO: Adding nullable CameraId is not so good but done for milestone 4
        public int? CameraId { get; set; }

        // Statistic #1: Stores the number of people identified within the second.
        public int NumTrackedPeople { get; set; }

        // Statistic #2: Indicates if the embedded system has stored an image locally of this second. This happens when a key statistic has occured. 
        public bool HasSavedImage { get; set; }
        
        public string FrameAsJpg { get; set; }
        
        public string FrameAsJpgPath { get; set; }

        #endregion

        public PerSecondStat()
        {
        }

        public PerSecondStat (string dateTime, string cameraKey, int numTrackedPeople, bool hasSavedImage)
        {
            this.DateTime = dateTime;
            this.CameraKey = cameraKey;
            this.NumTrackedPeople = numTrackedPeople;
            this.HasSavedImage = hasSavedImage;
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
            return (other.DateTime == DateTime && CameraKey == other.CameraKey);
        }
    }
}
