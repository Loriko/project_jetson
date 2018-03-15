using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Contains the body to be serialized to JSON format. This body is an object containing an array 
// of attributes of the PerSecondStats objects in a DataMessage detected as invalid. 
namespace WebAPI.Error_Response_Classes
{
    /// <summary>
    /// Class used to create the body of the error response for an invalid Data Message.
    /// </summary>
    public class InvalidDataMessageResponseBody
    {
        string Message;
        string[] InvalidAttributes; 

        public InvalidDataMessageResponseBody(string[] InvalidAttributes)
        {
            this.Message = "There is a problem with the data you provided. Please verify your DataMessage object and its contents.The following attributes were detected as invalid.";
            this.InvalidAttributes = InvalidAttributes;
        }
    }
}
