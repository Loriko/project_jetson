namespace BackEndServer.Classes.ErrorResponseClasses
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
