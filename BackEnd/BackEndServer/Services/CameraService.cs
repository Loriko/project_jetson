using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using BackEndServer.Services.PlaceholderServices;

namespace BackEndServer.Services
{
    public class CameraService : AbstractCameraService
    {
        private readonly DatabaseQueryService _dbQueryService;
        private readonly AbstractGraphStatisticService _graphStatisticsService;

        public CameraService(DatabaseQueryService dbQueryService, AbstractGraphStatisticService graphStatisticService)
        {
            _dbQueryService = dbQueryService;
            _graphStatisticsService = graphStatisticService;
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
                CameraStatistics cameraStatistics = new CameraStatistics()
                {
                    CameraInformation = new CameraInformation(camera),
                    DayTimeOfTheWeekAverageCount = 0,
                    DayTimeOfTheWeekAverageCountAvailable = false,
                    DayTimeOfTheWeekAverageCountDisplayString = null,
                    LastUpdatedTime = null,
                    MostRecentPeopleCount = 0,
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
                return cameraStatistics;
            }
            else
            {
                return null;
            }
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
    }
}