using System.Collections.Generic;

namespace BackEndServer.Models.ViewModels
{
    public class CameraRegistrationDetails
    {
        public CameraDetails CameraDetails { get; set; }
        public List<string> resolutions { get; set; }
        public LocationInformationList locations { get; set; }
    }
}