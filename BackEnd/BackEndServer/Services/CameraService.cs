using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services
{
    public class CameraService : AbstractCameraService
    {
        private readonly DatabaseQueryService _dbQueryService = new DatabaseQueryService();
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
                    PeriodOfTheDayAverageCountDisplayString = null
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
    }
}