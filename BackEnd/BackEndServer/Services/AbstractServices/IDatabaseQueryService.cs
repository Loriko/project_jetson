using System;
using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Services
{
    public interface IDatabaseQueryService
    {
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
    }
}