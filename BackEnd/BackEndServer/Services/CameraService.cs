using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.PlaceholderServices;
using BackEndServer.Services.HelperServices;
using System;
using System.IO;
using System.Threading.Tasks;
using BackEndServer.Models.Enums;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BackEndServer.Services
{
    public class CameraService : AbstractCameraService
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IDatabaseQueryService _dbQueryService;
        private readonly AbstractGraphStatisticService _graphStatisticsService;
        private readonly AbstractLocationService _locationService;
        private AbstractCameraService _abstractCameraServiceImplementation;

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

        public CameraKeyList GetCameraKeyListForAdmin()
        {
            List<DatabaseCamera> dbCameraList = _dbQueryService.GetAllCameras();
            return new CameraKeyList(dbCameraList);
        }

        public NewCameraKey GenerateUniqueCameraKey()
        {
            bool keyGenerated = false;

            while (keyGenerated == false)
            {
                // Camera Key must be exactly 12 characters.
                string randomCameraKey = StringGenerator.GenerateRandomString(12, 12);

                // Ensure Key does not exist in database (return value is -1).
                if (_dbQueryService.GetCameraIdFromKey(randomCameraKey) == -1)
                {
                    // Persist new camera key to database.

                    DatabaseCamera emptyCamera = new DatabaseCamera();
                    emptyCamera.CameraKey = randomCameraKey;
                    bool success = _dbQueryService.PersistNewCamera(emptyCamera);

                    if (success)
                    {
                        return new NewCameraKey(randomCameraKey);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        public bool DeleteCameraFromKey(string cameraKey)
        {
            return _dbQueryService.DeleteCameraFromCameraKey(cameraKey);
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

        //TODO: Refactor this method, a lot of things aren't used anymore
        public CameraDetails GetCameraInfoById(int cameraId)
        {
            DatabaseCamera camera = _dbQueryService.GetCameraById(cameraId);
            return new CameraDetails(camera);
        }

        public CameraStatistics getCameraStatisticsForNowById(int cameraId)
        {
            DatabaseCamera camera = _dbQueryService.GetCameraById(cameraId);
            DatabaseRoom room = null;
            if (camera != null && camera.RoomId != null)
            {
                room = _dbQueryService.GetRoomById(camera.RoomId.Value);
            }
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
                if (room != null)
                {
                    cameraStatistics.CameraDetails.MonitoredArea = room.RoomName;
                    cameraStatistics.CameraInformation.CameraRoomName = room.RoomName;
                    cameraStatistics.CameraInformation.RoomId = room.RoomId;
                }

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
            GraphStatistics graphStatistics = _graphStatisticsService.GetYearlyGraphStatistics(cameraId);
//            GraphStatistics graphStatistics = new PlaceholderGraphStatisticsService().GetYearlyGraphStatistics(cameraId);
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
                locations = _locationService.GetAvailableLocationsForUser(userId),
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
            try
            {
                if (cameraDetails.UploadedImage == null || PerformCameraImageUpload(cameraDetails))
                {
                    if (cameraDetails.ExistingRoomId <= 0)
                    {
                        DatabaseRoom dbRoom = new DatabaseRoom(cameraDetails);
                        if (!_dbQueryService.PersistNewRoom(dbRoom))
                        {
                            return false;
                        }
                        cameraDetails.ExistingRoomId = _dbQueryService.GetRoomIdByLocationIdAndRoomName(dbRoom.LocationId, dbRoom.RoomName);
                    }
                    return _dbQueryService.PersistExistingCameraByCameraKey(new DatabaseCamera(cameraDetails), cameraDetails.ImageDeleted);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return false;
        }

        private bool PerformCameraImageUpload(CameraDetails cameraDetails)
        {
            IFormFile image = cameraDetails.UploadedImage;

            // If the user has uploaded a file.
            if (image != null)
            {
                // Verify file size, must be under 5 MB.
                if (image.Length > 5000000)
                {
                    return false;
                }

                // Verify that the file is a valid image file (respects Minimum Size, File Extension and MIME Types).
                if (HttpPostedFileBaseExtensions.IsImage(image))
                {
                    // Proceed to process the request with the valid image.

                    // Obtain the file extension.
                    string fileExtension = Path.GetExtension(image.FileName).ToLower();

                    // Obtain the Database ID of the camera.
                    int cameraId = GetExistingCameraId(cameraDetails.CameraKey);

                    // Save the file to disk.

                    // 1. Ensure the output folder exists.
                    DirectoryInfo outputDirectory = Directory.CreateDirectory(DatabaseCamera.PATH_FOR_USER_UPLOADED_IMAGES);

                    // 2. Create the full file path (output path + filename).
                    string fullFilePath = Path.Combine(outputDirectory.FullName, cameraId + fileExtension);
                    cameraDetails.SavedImagePath = fullFilePath;

                    // 3. Save IFormFile as an image file in the output path.
                    using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        // NOTE: If this was for the Edit page, we would have to delete the previous picture first.
                        Task task = image.CopyToAsync(fileStream);
                        task.GetAwaiter().GetResult();
                    }
                    return true;
                }
            }

            return false;
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
            CameraRegistrationDetails registrationDetails = new CameraRegistrationDetails
            {
                locations = _locationService.GetAvailableLocationsForUser(userId),
                CameraDetails = new CameraDetails(_dbQueryService.GetCameraById(cameraId)),
                resolutions = GetExistingCameraResolutions()
            };
            if (!registrationDetails.CameraDetails.SavedImagePath.IsNullOrEmpty())
            {
                registrationDetails.CameraDetails.SavedImagePath = CameraRegistrationDetails.IMAGE_UPLOADED_TEXT;
            }

            return registrationDetails;
        }

        public CameraInformation GetCameraInformationForPastPeriod(int cameraId, PastPeriod pastPeriod, DateTime? startDate = null, DateTime? endDate = null)
        {
            CameraInformation cameraInformation = getCameraInformationById(cameraId);
            GraphStatistics graphStatistics = _graphStatisticsService.GetStatisticsForPastPeriod(cameraId, pastPeriod, startDate, endDate);
            graphStatistics.SelectedPeriod = pastPeriod;
            cameraInformation.GraphStatistics = graphStatistics;
            return cameraInformation;
        }

        public CameraInformationList GetAllCamerasInRoom(int roomId)
        {
            List<DatabaseCamera> dbCameras = _dbQueryService.GetAllCamerasInRoom(roomId);
            return new CameraInformationList(dbCameras);
        }

        public SharedGraphStatistics GetSharedRoomGraphStatistics(int roomId)
        {
            CameraInformationList camerasInRoom = GetAllCamerasInRoom(roomId);
            RoomInfo roomInfo = new RoomInfo(_dbQueryService.GetRoomById(roomId));
            return new SharedGraphStatistics
            {
                DisplayedCameras = camerasInRoom,
                Room = roomInfo
            };
        }
        
        public List<DatabaseUser> GetAllUsers()
        {
            List<DatabaseUser> dbUserList = _dbQueryService.GetAllUsers();
            
            return dbUserList;
        }

        public bool GiveAccessToUser(int cameraId, int userId)
        {
            bool value = _dbQueryService.CreateUserCameraAssociation(userId, cameraId);
            return value;
        }
    }
}