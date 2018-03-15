using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonGenerator.Classes
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
    }
}