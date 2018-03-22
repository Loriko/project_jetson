namespace BackEndServer.Classes.ErrorResponseClasses
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