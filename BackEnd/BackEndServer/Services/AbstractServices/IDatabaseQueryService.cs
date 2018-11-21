using System;
using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.APIModels;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface IDatabaseQueryService
    {
        //TODO check if this can be deleted
        string ConnectionString { get; set; }

        // PERSIST methods:
        bool PersistNewPerSecondStats(List<DatabasePerSecondStat> distinctStats);
        bool PersistNewPerHourStats(List<DatabasePerHourStat> perHourStats);
        bool PersistNewCamera(DatabaseCamera camera);
        bool PersistNewAPIKey(APIKey api_key);
        bool PersistNewRoom(DatabaseRoom databaseRoom);
        bool PersistNewAlert(DatabaseAlert alert);
        bool PersistExistingAlert(DatabaseAlert alert);
        bool PersistNewLocation(DatabaseLocation dbLocation);
        bool PersistExistingUser(DatabaseUser databaseUser);
        bool PersistPasswordChange(DatabaseUser databaseUser);
        bool PersistNewUser(DatabaseUser databaseUser);
        bool PersistNewNotification(DatabaseNotification dbNotification);
        bool PersistExistingCameraByCameraKey(DatabaseCamera databaseCamera, bool imageDeleted);

        // UPDATE methods:
        bool UpdatePerSecondStatsWithPerHourStatId(DateTime hour, int perHourStatId);
        int? GetUserIdByUsername(string username);

        // DELETE methods:
        bool DeleteCameraFromCameraKey(string cameraKey);

        // SELECT methods:

        // For Camera:
        DatabaseCamera GetCameraById(int cameraId);
        DatabaseCamera GetCameraByKey(string cameraKey);
        List<DatabaseCamera> GetCamerasForLocation(int locationId);
        List<DatabaseCamera> GetAllCameras();
        List<string> GetAllCameraKeys(); 
        List<string> GetExistingCameraResolutions();
        int GetCameraIdFromKey(string cameraKey);
        int GetAPIKeyIdFromKey(string apiKey);
        string GetCameraKeyFromId(int cameraId);

        // For PerSecondStat:
        DatabasePerSecondStat GetLatestPerSecondStatForCamera(int cameraId);
        List<DatabasePerSecondStat> GetPerSecondStatsForCamera(int cameraId);
        List<DatabasePerSecondStat> GetStatsFromInterval(TimeInterval verifiedTimeInterval);
        List<DatabasePerSecondStat> GetAllSecondsForHourForCamera(DateTime dateTime, int cameraId);

        // For Location:
        List<DatabaseLocation> GetLocationsForUser(int userId);
        List<DatabaseLocation> GetLocationsCreatedByUser(int userId);
        // For User:
        bool IsPasswordValidForUser(string username, string password);
        List<DatabaseCamera> GetCamerasAvailableToUser(int userId);
        List<DatabaseAlert> GetAllAlerts(int userId = 0);
        bool DeleteAlert(int alertId);
        DatabasePerSecondStat GetEarliestPerSecondStatTriggeringAlert(DatabaseAlert alert, DateTime lastUpdatedTime,
            DateTime checkupDateTime);

        List<DatabasePerSecondStat> GetPerSecondStatsWithFrmTriggeringAlert(DatabaseAlert alert,
            DateTime lastUpdatedTime, DateTime checkupDateTime);
        List<DatabaseNotification> GetNotificationsForUser(int userId);
        List<DatabaseAlert> GetAlertsById(List<int> alertIds);
        DatabaseNotification GetNotificationById(int notificationId);
        bool AcknowledgeNotification(int notificationId);
        DatabaseAlert GetAlertById(int alertId);
        DatabaseLocation GetLocationById(int locationId);
        List<DatabaseCamera> GetCamerasOwnedByUser(int userId);
        List<DatabaseCamera> GetCamerasForLocationForUser(int locationId, int userId);
        DatabaseUser GetUserById(int userId);
        bool IsUserAdministrator(int userId);
        DatabaseUser GetUserByEmailAddress(string emailAddress);
        DatabaseUser GetUserByPasswordResetToken(string token);
        bool PersistPasswordResetToken(string passwordResetToken, string emailAddress);
        bool PersistRemovePasswordResetToken(string resetToken);
        DatabaseUser GetUserByUsername(string username);
        DatabaseGraphStat getGraphStatByTimeInterval(int cameraID, DateTime start, DateTime end);
        List<DatabaseLocation> GetLocations();
        List<DatabaseRoom> GetRoomsAtLocation(int locationId);
        int GetRoomIdByLocationIdAndRoomName(int locationId, string roomName);
        DatabaseRoom GetRoomById(int cameraRoomId);
        List<DatabaseCamera> GetAllCamerasInRoom(int roomId);
        List<DatabaseUser> GetAllUsers();
        bool CreateUserCameraAssociation(int userId, int cameraId);
        List<DatabaseUserCameraAssociation> GetAllUserCameraAssociations();
        DatabaseCamera GetCameraWithNameAtLocation(int locationId, string cameraName);
        bool DeleteAlertsWithCameraId(int cameraId);
        bool DeletePerSecondStatsWithCameraId(int cameraId);
        bool DeleteRoomsAtLocation(int locationId);
        bool DeleteLocation(int locationId);
        bool PersistExistingLocation(DatabaseLocation dbLocation);
        List<DatabaseAlert> GetAlertsByCameraId(int cameraId);
        bool DeleteUserCameraAssociation(DatabaseUserCameraAssociation dbAssociation);
        List<DatabaseUser> GetUsersWithCameraViewAccess(int cameraId);
    }
}