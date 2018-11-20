namespace BackEndServer.Classes.ErrorResponseClasses
{
    public class InvalidAveragesOfDayRequestResponseBody
    {
#pragma warning disable 414
        string Message;
#pragma warning restore 414

        public InvalidAveragesOfDayRequestResponseBody()
        {
            this.Message = "Invalid SingleSecondTime object provided in the request for the hourly averages of a day";
        }
    }
}
