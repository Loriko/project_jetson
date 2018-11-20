namespace BackEndServer.Classes.ErrorResponseClasses
{
    public class InvalidLocationIdResponseBody
    {
#pragma warning disable 414
        string Message;
#pragma warning restore 414

        public InvalidLocationIdResponseBody()
        {
            this.Message = "The Location ID provided is invalid. Please ensure the ID is a positive integer.";
        }
    }
}