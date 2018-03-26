// Contains the body to be serialized to JSON format. This body is an object containing an array 
// of attributes of the PerSecondStat objects in a DataMessage detected as invalid. 
namespace BackEndServer.Classes.ErrorResponseClasses
{
    /// <summary>
    /// Class used to create the body of the error response for an invalid Data Message.
    /// </summary>
    public class InvalidDataMessageResponseBody
    {
        string Message;
        string[] InvalidAttributes; 

        public InvalidDataMessageResponseBody(bool receivedEmptyMessage, string[] invalidAttributes)
        {
            if (receivedEmptyMessage)
            {
                this.Message = "You provided an empty DataMessage object. Please verify your DataMessage object and its contents.";
                this.InvalidAttributes = null;
            }
            else
            {
                this.Message = "There is a problem with the data you provided. Please verify your DataMessage object and its contents.The following attributes were detected as invalid.";
                this.InvalidAttributes = invalidAttributes;
            }
        }
    }
}
