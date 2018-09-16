using System;
using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface IDatabaseQueryService
    {
        //TODO check if I can delete this
        string ConnectionString { get; set; }

        // PERSIST methods:
        bool PersistNewPerSecondStats(List<PerSecondStat> distinctStats);
        bool PersistNewPerHourStats(List<DatabasePerHourStat> perHourStats);
        bool PersistNewCamera(DatabaseCamera camera);

        // UPDATE methods:
        bool UpdatePerSecondStatsWithPerHourStatId(DateTime hour, int perHourStatId);

        // QUERY methods:

        // For Camera:
        DatabaseCamera GetCameraById(int cameraId);
        List<DatabaseCamera> GetCamerasForLocation(int locationId);
        List<string> GetExistingCameraResolutions();
        // For PerSecondStat:
        DatabasePerSecondStat GetLatestPerSecondStatForCamera(int cameraId);
        List<DatabasePerSecondStat> GetPerSecondStatsForCamera(int cameraId);
        List<DatabasePerSecondStat> GetStatsFromInterval(TimeInterval verifiedTimeInterval);
        List<DatabasePerSecondStat> GetAllSecondsForHourForCamera(DateTime dateTime, int cameraId);
        // For PerHourStat:
        DatabasePerHourStat GetPerHourStatFromHour(DateTime hour);
        // For Location:
        List<DatabaseLocation> GetLocationsForUser(string username);
        // For User:
        bool IsPasswordValidForUser(string username, string password);
        List<DatabaseCamera> GetCamerasAvailableToUser(int userId);
        bool PersistNewAlert(DatabaseAlert alert);
        List<DatabaseAlert> GetAllAlerts(int userId = 0);
        bool DeleteAlert(int alertId);
        bool PersistExistingAlert(DatabaseAlert alert);
        bool PersistNewLocation(DatabaseLocation dbLocation);
        DatabasePerSecondStat GetEarliestPerSecondStatTriggeringAlert(DatabaseAlert alert, DateTime lastUpdatedTime,
            DateTime checkupDateTime);
        List<DatabaseNotification> GetNotificationsForUser(int userId);
        List<DatabaseAlert> GetAlertsById(List<int> alertIds);
        DatabaseNotification GetNotificationById(int notificationId);
        bool AcknowledgeNotification(int notificationId);
        DatabaseAlert GetAlertById(int alertId);
        DatabaseLocation GetLocationById(int locationId);
        bool PersistNewNotification(DatabaseNotification dbNotification);
        bool PersistExistingCameraByCameraKey(DatabaseCamera databaseCamera);
    }
}