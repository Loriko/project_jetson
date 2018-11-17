namespace BackEndServer.Models.ViewModels
{
    public class PostRequestResult
    {
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public bool NextLocation { get; set; }
    }
}