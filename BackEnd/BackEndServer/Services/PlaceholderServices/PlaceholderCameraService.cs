using System;
using BackEndServer.Models.ViewModels;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services.PlaceholderServices
{
    public class PlaceholderCameraService : AbstractCameraService
    {
        public CameraInformationList getCamerasAtLocation(int locationId){
            if (locationId == 1 || locationId == 2 || locationId == 3 || locationId == 4){
                List<CameraInformation> camerasAtLocation = new List<CameraInformation>();
                camerasAtLocation.Add(new CameraInformation(1, "East Waiting Room"));
                camerasAtLocation.Add(new CameraInformation(2, "West Waiting Room"));
                camerasAtLocation.Add(new CameraInformation(3, "Written Test Room"));
                camerasAtLocation.Add(new CameraInformation(4, "Entrance"));
                CameraInformationList camerasAtLocationModel =
                    new CameraInformationList {CameraList = camerasAtLocation};
                return camerasAtLocationModel;
            }
            else {
                return null;
            }
        }

        public List<DatabaseCamera> getDatabaseCamerasAtLocation(int locationId)
        {
            throw new NotImplementedException(
                "This is a temporary method that shouldn't exist in the first place, so no placeholder was created.");
        }

        public CameraInformation GetCameraInformationWithYearlyData(int cameraId)
        {
            throw new NotImplementedException();
        }

        public CameraInformation getCameraInformationById(int cameraId){
            if (cameraId == 1)
            {
                return new CameraInformation(1, "East Waiting Room");
            }
            else if (cameraId == 2)
            {
                return new CameraInformation(2, "West Waiting Room");
            }
            else if (cameraId == 3)
            {
                return new CameraInformation(3, "Written Test Room");
            }
            else if (cameraId == 4)
            {
                return new CameraInformation(4, "Entrance");
            }
            else 
            {
                return null;    
            }
        }

        public CameraStatistics getCameraStatisticsForNowById(int cameraId)
        {
            CameraInformation cameraInformation = this.getCameraInformationById(cameraId);
            if (cameraInformation != null)
            {
                CameraStatistics cameraStatisticsModel = new CameraStatistics();
                cameraStatisticsModel.CameraInformation = cameraInformation;
                // In this placeholder model, we always use the same statistics
                cameraStatisticsModel.LastUpdatedTime = new DateTime(2018, 2, 18, 14, 11, 2);
                cameraStatisticsModel.MostRecentPeopleCount = 21;
                cameraStatisticsModel.DayTimeOfTheWeekAverageCount = 30;
                cameraStatisticsModel.DayTimeOfTheWeekAverageCountDisplayString = "On a Friday between 2:00 PM and 3:00 PM";
                cameraStatisticsModel.PeriodOfTheDayAverageCount = 25;
                cameraStatisticsModel.PeriodOfTheDayAverageCountDisplayString = "On an average afternoon";
                return cameraStatisticsModel;
            }
            else {
                return null;
            }
        }

        public bool SaveNewCamera(CameraDetails cameraDetails)
        {
            return true;
        }

        public CameraRegistrationDetails GetNewCameraRegistrationDetails(string username)
        {
            throw new NotImplementedException("GetNewCameraRegistrationDetails placeholder not implemented!");
        }
    }
}
