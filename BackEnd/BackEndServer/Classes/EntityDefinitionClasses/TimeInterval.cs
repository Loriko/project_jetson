using System;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Classes.EntityDefinitionClasses
{
    /// <summary>
    /// Represents a DateTime interval using strings in MYSQL DateTime format.
    /// </summary>
    public class TimeInterval
    {
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }

        // Constructor required only for ASP.Net Core's automatic Json deserialization.
        // Web API will not need to create TimeInterval objects.
        public TimeInterval(string startDateTime, string endDateTime)
        {
            this.StartDateTime = startDateTime;
            this.EndDateTime = endDateTime;
        }
    }
}