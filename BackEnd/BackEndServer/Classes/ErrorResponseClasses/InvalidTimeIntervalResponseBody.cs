namespace BackEndServer.Classes.ErrorResponseClasses
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