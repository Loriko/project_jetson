using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Services
{
    public interface IDatabaseQueryService
    {
        string ConnectionString { get; set; }

        DatabaseCamera GetCameraById(int cameraId);
        List<DatabaseCamera> GetCamerasForLocation(int locationId);
        DatabasePerSecondStat GetLatestPerSecondStatForCamera(int cameraId);
        List<DatabaseLocation> GetLocationsForUser(string username);
        List<DatabasePerSecondStat> GetPerSecondStatsForCamera(int cameraId);
        List<DatabasePerSecondStat> getStatsFromInterval(TimeInterval verifiedTimeInterval);
        bool IsPasswordValidForUser(string username, string password);
        bool storePerSecondStats(List<PerSecondStat> distinctStats);
        List<TestObject> testDatabase();
    }
}