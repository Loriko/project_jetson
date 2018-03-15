using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Error_Response_Classes
{
    public class InvalidAveragesOfDayRequestResponseBody
    {
        string Message;

        public InvalidAveragesOfDayRequestResponseBody()
        {
            this.Message = "Invalid SingleSecondTime object provided in the request for the hourly averages of a day";
        }
    }
}
