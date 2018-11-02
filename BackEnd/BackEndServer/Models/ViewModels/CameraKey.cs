namespace BackEndServer.Models.ViewModels
{
    public class CameraKey
    {
        public int CameraId { get; set; }
        public string Key { get; set; }
        public bool IsRegistered { get; set; }

        public CameraKey(int cameraId, string key, bool isRegistered)
        {
            CameraId = cameraId;
            Key = key;
            IsRegistered = isRegistered;
        }
    }
}
