using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Error_Response_Classes
{
    public class InvalidLocationIdResponseBody
    {
        string Message;

        public InvalidLocationIdResponseBody()
        {
            this.Message = "The Location ID provided is invalid. Please ensure the ID is a positive integer.";
        }
    }
}