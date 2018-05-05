using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Services.HelperServices;
using Newtonsoft.Json;

namespace BackEndServer.Classes.DataRequestClasses
{
    /// <summary>
    /// Represents a request for the hourly averages of an entire day. Simplified without time zones.
    /// </summary>
    public class AveragesOfDayRequest
    {
        public string DateTime;

        [JsonConstructor]
        public AveragesOfDayRequest(string dateTime)
        {
            this.DateTime = dateTime;
        }

        /// <summary>
        /// Method to validate a AveragesOfDayRequest object by checking if the requested day is valid.
        /// </summary>
        /// <returns>Returns TRUE if the date is valid, returns FALSE if date is not valid.</returns>
        public bool IsValidRequest()
        {
            return (DateTimeTools.ValidateDateTimeString(this.DateTime));
        }
    }
}