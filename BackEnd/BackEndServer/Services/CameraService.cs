﻿using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.PlaceholderServices;
using BackEndServer.Services.HelperServices;
using System;
using System.IO;

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

        public List<DatabaseCamera> GetDatabaseCamerasAtLocation(int locationId)
        {
            List<DatabaseCamera> dbCameraList = _dbQueryService.GetCamerasForLocation(locationId);
            return dbCameraList;
        }

        public CameraInformationList GetCamerasAtLocationForUser(int locationId, int userId)
        {
            List<DatabaseCamera> dbCameraList = _dbQueryService.GetCamerasForLocationForUser(locationId, userId);

            CameraInformationList listOfCameraInfo = new CameraInformationList(dbCameraList);

            return InitialiseImagesBeforeDisplaying(listOfCameraInfo);
        }

        private CameraInformationList InitialiseImagesBeforeDisplaying(CameraInformationList list)
        {
            foreach(CameraInformation camInfo in list.CameraList)
            {
                camInfo.TempImagePath = GenerateTempImageAndTempPath(camInfo.ImagePath);
            }

            return list;
        }

        private string GenerateTempImageAndTempPath(string imagePath)
        {
            // If the camera has a value in the image_path column of the Camera table in the database, if not a question mark will be displayed instead.
            if (String.IsNullOrWhiteSpace(imagePath) == false)
            {
                // Ensure the image file still exists on the server, if not a question mark will be displayed instead.
                if (File.Exists(imagePath))
                {
                    /*
                     NOTE: To create/delete/overwrite file in the wwwroot directory, physical file paths must be used
                            and not virual paths (using ~). Use RootDirectoryTools.cs in HelperServices.
                    */

                    // 1. Extract the FileName from the path stored in the database.
                    string fileName = Path.GetFileName(imagePath);

                    // 2. Create the full file output path (physical).
                    string tempPath = Path.Combine(RootDirectoryTools.GetWWWRootPhysicalPath(), fileName);

                    // 3. Delete any previous file in the temp folder associated to the camera.
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }

                    // 4. Copy the image from the path in the "...\BackEnd\BackEndServer\ImageStorage\CameraImages" to the phycal wwwroot path's "\temp" folder.
                    File.Copy
                    (
                        // Source Image Path (stored in the ImageStorage\CameraImages folder in the project.)
                        imagePath,
                        // Write to the wwwroot directory's temp path (physical), output filename remains the same. 
                        Path.Combine(RootDirectoryTools.GetWWWRootTempFolderPhysicalPath(), fileName),
                        // If file was not deleted properly, ensure the existing file is overwritten.
                        true
                    );

                    // 5. Set the View Model object's "TempImagePath" attribute to the appropriate 
                    // value for use in the img element's src attribute to locate the image in the wwwroot.
                    return String.Concat(RootDirectoryTools.GetWWWRootTempFolderVirtualPathForHTML(), "/", fileName);
                }
            }

            return null;
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
                    GraphStatistics = graphStatistics,
                    TempImagePath = null 
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

                cameraStatistics.TempImagePath = GenerateTempImageAndTempPath(camera.ImagePath);

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

        public CameraRegistrationDetails GetNewCameraRegistrationDetails(int userId)
        {
            return new CameraRegistrationDetails()
            {
                locations = _locationService.getAvailableLocationsForUser(userId),
                CameraDetails = new CameraDetails(),
                resolutions = GetExistingCameraResolutions()
            };
        }

        public CameraInformationList GetAllCamerasOwnedByUser(int userId) {
            List<DatabaseCamera> dbCameras = _dbQueryService.GetCamerasOwnedByUser(userId);
            return new CameraInformationList(dbCameras);
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
            return _dbQueryService.PersistExistingCameraByCameraKey (new DatabaseCamera(cameraDetails));
        }

        public List<string> GetExistingCameraResolutions()
        {
            return _dbQueryService.GetExistingCameraResolutions();
        }

        public int GetExistingCameraId(string cameraKey)
        {
            return _dbQueryService.GetCameraIdFromKey(cameraKey);
        }

        public CameraRegistrationDetails GetCameraRegistrationDetailsById(int cameraId, int userId)
        {
            return new CameraRegistrationDetails
            {
                locations = _locationService.getAvailableLocationsForUser(userId),
                CameraDetails = new CameraDetails(_dbQueryService.GetCameraById(cameraId)),
                resolutions = GetExistingCameraResolutions()
            };
        }
    }
}