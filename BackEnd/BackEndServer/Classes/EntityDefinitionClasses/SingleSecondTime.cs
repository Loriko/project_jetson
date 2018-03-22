using System;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// This class also acts as a SingleSecondTimeRequest class.
    /// </summary>
    public class SingleSecondTime
    {
        public long UnixTime { get; set; }

        public SingleSecondTime(long UnixTime)
        {
            this.UnixTime = UnixTime;
        }

        public bool isValidSingleSecondTime()
        {
            // Convert from Unix Time to a Date Time object.
            DateTime time = this.UnixTime.toDateTime();

            // Validate Date Time object and return validation result (valid or invalid).
            return (DateTimeTools.validateDateTime(time)); 
        }
    }
}