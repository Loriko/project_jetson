namespace BackEndServer.Classes.ErrorResponseClasses
{
    public class FailedPersistResponseBody
    {
#pragma warning disable 414
        string Message;
#pragma warning restore 414

        public FailedPersistResponseBody()
        {
            this.Message = "Failed Persist: Unable to persist received data to the database.";
        }
    }
}
