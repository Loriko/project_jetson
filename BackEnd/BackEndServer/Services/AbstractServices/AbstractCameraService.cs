using System;
using System.Collections.Generic;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractCameraService
    {
        CameraInformationList getCamerasAtLocation(int locationId);
        CameraInformation getCameraInformationById(int cameraId);
        CameraStatistics getCameraStatisticsForNowById(int cameraId);
    }
}
