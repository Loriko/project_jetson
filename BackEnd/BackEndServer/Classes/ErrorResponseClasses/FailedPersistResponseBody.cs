namespace BackEndServer.Classes.ErrorResponseClasses
{
    public class FailedPersistResponseBody
    {
        string Message;

        public FailedPersistResponseBody()
        {
            this.Message = "Failed Persist: Unable to persist received data to the database.";
        }
    }
}
