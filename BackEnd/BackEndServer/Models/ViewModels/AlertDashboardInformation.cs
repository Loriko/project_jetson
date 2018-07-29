using System.Collections.Generic;

namespace BackEndServer.Models.ViewModels
{
    public class AlertDashboardInformation
    {
        public CameraInformationList Availablecameras { get; set; }
        public SortedDictionary<int, List<AlertDetails>> ExistingAlertsByCameraId { get; set; }

        public AlertDashboardInformation()
        {
        }
    }
}