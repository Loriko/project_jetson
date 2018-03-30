namespace BackEndServer.Classes.ErrorResponseClasses
{
    public class InvalidTimeIntervalResponseBody
    {
        string Message;

        public InvalidTimeIntervalResponseBody()
        {
            this.Message = "Invalid TimeInterval object provided. Please verify both start and end DateTime attributes and ensure they are in this MySQL format: '9999-12-31 23:59:59'";
        }
    }
}