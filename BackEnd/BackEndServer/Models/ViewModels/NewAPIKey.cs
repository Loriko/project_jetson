namespace BackEndServer.Models.ViewModels
{
    public class NewAPIKey
    {
        public string PlainTextAPIKey { get; set; }

        public NewAPIKey(string plainTextKey)
        {
            PlainTextAPIKey = plainTextKey;
        }
    }
}