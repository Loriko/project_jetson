using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractCameraService
    {
        CameraInformationList getCamerasAtLocation(int locationId);
        CameraInformation getCameraInformationById(int cameraId);
        CameraStatistics getCameraStatisticsForNowById(int cameraId);
        // Temporary, needs to be changed to using the APIModel equivalent of the DatabaseCamera object
        List<DatabaseCamera> getDatabaseCamerasAtLocation(int locationId);
        CameraInformation GetCameraInformationWithYearlyData(int cameraId);
        bool SaveNewCamera(CameraDetails cameraDetails);
        CameraRegistrationDetails GetNewCameraRegistrationDetails(string username);
        CameraInformationList getAllCamerasForUser(int userId);
        CameraInformationList GetAllCamerasOwnedByUser(int userId);
        List<CameraInformation> getAllCamerasForUser(string username);
        bool RegisterCamera(CameraDetails cameraDetails);
    }
}
