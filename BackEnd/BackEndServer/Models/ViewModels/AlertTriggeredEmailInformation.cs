using System;

namespace BackEndServer.Models.ViewModels
{
    public class AlertTriggeredEmailInformation
    {
        public string Username;
        public string AlertName;
        public string CameraName;
        public string LocationName;
        public DateTime DateTriggered;

        public AlertTriggeredEmailInformation()
        {
        }
    }
}