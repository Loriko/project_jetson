namespace BackEndServer.Models.ViewModels
{
    public class CameraKey
    {
        public string Key { get; set; }
        public bool IsRegistered { get; set; }

        public CameraKey(string key, bool isRegistered)
        {
            Key = key;
            IsRegistered = isRegistered;
        }
    }
}
