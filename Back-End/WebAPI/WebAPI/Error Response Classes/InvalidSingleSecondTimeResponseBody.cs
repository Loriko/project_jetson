using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Error_Response_Classes
{
    public class InvalidSingleSecondTimeResponseBody
    {
        string Message;

        public InvalidSingleSecondTimeResponseBody()
        {
            this.Message = "The SingleSecondTime object you provided is invalid. Please verify the attributes of that object.";
        }
    }
}
