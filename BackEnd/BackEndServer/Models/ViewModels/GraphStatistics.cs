using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndServer.Models.ViewModels
{
    public class GraphStatistics
    {
        public string[][] Stats { get; set; }
        public CameraInformation CameraInformation { get; set; }

        public GraphStatistics() { }

        public GraphStatistics(CameraInformation cameraInformation)
        {
            CameraInformation = cameraInformation;
        }
    }
}
