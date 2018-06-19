using BackEndServer.Services.HelperServices;
using Newtonsoft.Json;

namespace BackEndServer.Classes.DataRequestClasses
{
    /// <summary>
    /// Represents a request for the hourly averages of an entire day. Simplified without time zones.
    /// </summary>
    public class AveragesOfDayRequest
    {
        // Attribute(s)
        public string API_Key { get; set; }
        public string Date;

        [JsonConstructor]
        public AveragesOfDayRequest(string api_key, string date)
        {
            this.API_Key = api_key;
            this.Date = date + " 01:00:00";
        }

        /// <summary>
        /// Method to validate a AveragesOfDayRequest object by checking if the requested day is valid.
        /// </summary>
        /// <returns>Returns TRUE if the date is valid, returns FALSE if date is not valid.</returns>
        public bool IsValidRequest()
        {
            return (DateTimeTools.ValidateDateTimeString(this.Date));
        }
    }
}