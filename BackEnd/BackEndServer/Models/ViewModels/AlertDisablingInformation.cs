using System;

namespace BackEndServer.Models.ViewModels
{
    public class AlertDisablingInformation
    {
        public int AlertId { get; set; }
        public bool DisableForever { get; set; }
        public DateTime? DisabledUntil { get; set; }

        public AlertDisablingInformation() {}
    }
}