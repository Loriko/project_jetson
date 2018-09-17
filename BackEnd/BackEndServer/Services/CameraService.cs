using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.PlaceholderServices;

namespace BackEndServer.Services
{
    public class CameraService : AbstractCameraService
    {
        private readonly IDatabaseQueryService _dbQueryService;
        private readonly AbstractGraphStatisticService _graphStatisticsService;
        private readonly AbstractLocationService _locationService;
        
        public CameraService(IDatabaseQueryService dbQueryService, AbstractGraphStatisticService graphStatisticService, AbstractLocationService locationService)
        {
            _dbQueryService = dbQueryService;
            _graphStatisticsService = graphStatisticService;
            _locationService = locationService;
        }

        public List<DatabaseCamera> getDatabaseCamerasAtLocation(int locationId)
        {
            List<DatabaseCamera> dbCameraList = _dbQueryService.GetCamerasForLocation(locationId);
            return dbCameraList;
        }

        public CameraInformationList getCamerasAtLocation(int locationId)
        {
            List<DatabaseCamera> dbCameraList = _dbQueryService.GetCamerasForLocation(locationId);
            return new CameraInformationList(dbCameraList);
        }

        public CameraInformation getCameraInformationById(int cameraId)
        {
            DatabaseCamera camera = _dbQueryService.GetCameraById(cameraId);
            return new CameraInformation(camera);
        }

        public CameraStatistics getCameraStatisticsForNowById(int cameraId)
        {
            DatabaseCamera camera = _dbQueryService.GetCameraById(cameraId);
            DatabasePerSecondStat mostRecentStat = _dbQueryService.GetLatestPerSecondStatForCamera(cameraId);
            GraphStatistics graphStatistics = _graphStatisticsService.GetLast30MinutesStatistics(cameraId);
            if (camera != null)
            {
                CameraStatistics cameraStatistics = new CameraStatistics
                {
                    CameraInformation = new CameraInformation(camera),
                    CameraDetails = new CameraDetails(camera),
                    DayTimeOfTheWeekAverageCount = 0,
                    DayTimeOfTheWeekAverageCountAvailable = false,
                    DayTimeOfTheWeekAverageCountDisplayString = null,
                    LastUpdatedTime = null,
                    MostRecentPeopleCount = null,
                    PeriodOfTheDayAverageCount = 0,
                    PeriodOfTheDayAverageCountAvailable = false,
                    PeriodOfTheDayAverageCountDisplayString = null,
                    GraphStatistics = graphStatistics
                };
                if (mostRecentStat != null)
                {
                    cameraStatistics.LastUpdatedTime = mostRecentStat.DateTime;
                    cameraStatistics.MostRecentPeopleCount = mostRecentStat.NumDetectedObjects;
                }
                if (camera.LocationId != null)
                {
                    cameraStatistics.CameraDetails.Location = new LocationDetails(_dbQueryService.GetLocationById(camera.LocationId.Value));
                }
                return cameraStatistics;
            }
            
            return null;
        }

        public CameraInformation GetCameraInformationWithYearlyData(int cameraId)
        {
            CameraInformation cameraInformation = getCameraInformationById(cameraId);
            // Line below is what it should be... but we're using the placeholder graphStatisticService for now
//            GraphStatistics graphStatistics = _graphStatisticsService.GetYearlyGraphStatistics(cameraId);
            GraphStatistics graphStatistics = new PlaceholderGraphStatisticsService().GetYearlyGraphStatistics(cameraId);
            cameraInformation.GraphStatistics = graphStatistics;
            return cameraInformation;
        }

        public bool SaveNewCamera(CameraDetails cameraDetails)
        {
            DatabaseCamera camera = new DatabaseCamera(cameraDetails);
            return _dbQueryService.PersistNewCamera(camera);
        }

        public CameraRegistrationDetails GetNewCameraRegistrationDetails(string username)
        {
            return new CameraRegistrationDetails()
            {
                locations = _locationService.getAvailableLocationsForUser(username),
                CameraDetails = new CameraDetails(),
                resolutions = GetExistingCameraResolutions()
            };
        }

        public CameraInformationList getAllCamerasForUser(int userId)
        {
            List<DatabaseCamera> dbCameras = _dbQueryService.GetCamerasAvailableToUser(userId);
            return new CameraInformationList(dbCameras);
        }

        public List<CameraInformation> getAllCamerasForUser(string username)
        {
            throw new System.NotImplementedException("getAllCamerasForUser(string username) is not implemented");
        }

        public bool RegisterCamera(CameraDetails cameraDetails)
        {
            return _dbQueryService.PersistExistingCameraByCameraKey(new DatabaseCamera(cameraDetails));
        }

        public List<string> GetExistingCameraResolutions()
        {
            return _dbQueryService.GetExistingCameraResolutions();
        }
    }
}