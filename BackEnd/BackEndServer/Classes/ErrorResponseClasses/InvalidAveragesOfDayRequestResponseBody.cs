namespace BackEndServer.Classes.ErrorResponseClasses
{
    public class InvalidAveragesOfDayRequestResponseBody
    {
        string Message;

        public InvalidAveragesOfDayRequestResponseBody()
        {
            this.Message = "Invalid SingleSecondTime object provided in the request for the hourly averages of a day";
        }
    }
}
