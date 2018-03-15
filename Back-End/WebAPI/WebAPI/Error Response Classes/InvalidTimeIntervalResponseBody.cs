using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Error_Response_Classes
{
    public class InvalidTimeIntervalResponseBody
    {
        string Message;

        public InvalidTimeIntervalResponseBody()
        {
            this.Message = "Invalid TimeInterval object provided. Please verify both Unix Time attributes and ensure they are valid.";
        }
    }
}