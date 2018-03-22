using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Classes.EntityDefinitionClasses;

namespace BackEndServer.Classes.DataRequestClasses
{
    /// <summary>
    /// Represents a request for the hourly averages of an entire day. Simplified without time zones.
    /// </summary>
    public class AveragesOfDayRequest
    {
        public SingleSecondTime SingleSecondTime;

        // Constructor, only added in order to enable proper deserialization. Web API does not create this request object.
        public AveragesOfDayRequest(SingleSecondTime SingleSecondTime)
        {
            this.SingleSecondTime = SingleSecondTime;
        }

        /// <summary>
        /// Method to validate a AveragesOfDayRequest object by checking if the requested day is valid.
        /// </summary>
        /// <returns>Returns TRUE if the date is valid, returns FALSE if date is not valid.</returns>
        public bool isValidRequest()
        {
            return (this.SingleSecondTime.isValidSingleSecondTime());
        }
    }
}