using System;
using BackEndServer.Models.Enums;

namespace BackEndServer.Models.ViewModels
{
    public class SharedGraphStatistics
    {
        public CameraInformationList DisplayedCameras { get; set; }
        public RoomInfo Room { get; set; }
        public PastPeriod SelectedPeriod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}