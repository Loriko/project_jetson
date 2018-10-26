using System;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.Enums;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractCameraService
    {
        CameraInformationList getCamerasAtLocation(int locationId);
        CameraKeyList GetCameraKeyListForAdmin();
        bool DeleteCameraFromKey(string cameraKey);
        NewCameraKey GenerateUniqueCameraKey();
        CameraInformation getCameraInformationById(int cameraId);
        CameraStatistics getCameraStatisticsForNowById(int cameraId);
        // Temporary, needs to be changed to using the APIModel equivalent of the DatabaseCamera object
        List<DatabaseCamera> GetDatabaseCamerasAtLocation(int locationId);
        CameraInformationList GetCamerasAtLocationForUser(int locationId, int userId);
        CameraInformation GetCameraInformationWithYearlyData(int cameraId);
        bool SaveNewCamera(CameraDetails cameraDetails);
        CameraRegistrationDetails GetNewCameraRegistrationDetails(int userId);
        CameraInformationList getAllCamerasForUser(int userId);
        CameraInformationList GetAllCamerasOwnedByUser(int userId);
        List<CameraInformation> getAllCamerasForUser(string username);
        bool RegisterCamera(CameraDetails cameraDetails);
        int GetExistingCameraId(string cameraKey);
        CameraRegistrationDetails GetCameraRegistrationDetailsById(int cameraId, int userId);
        CameraInformation GetCameraInformationForPastPeriod(int cameraId, PastPeriod pastPeriod, DateTime? startDate = null, DateTime? endDate = null);
    }
}
