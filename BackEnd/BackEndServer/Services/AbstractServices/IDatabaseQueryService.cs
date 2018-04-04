﻿using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface IDatabaseQueryService
    {
        string ConnectionString { get; set; }

        DatabaseCamera GetCameraById(int cameraId);
        List<DatabaseCamera> GetCamerasForLocation(int locationId);
        DatabasePerSecondStat GetLatestPerSecondStatForCamera(int cameraId);
        List<DatabaseAddress> GetLocationsForUser(string username);
        List<DatabasePerSecondStat> getStatsFromInterval(TimeInterval verifiedTimeInterval);
        bool IsPasswordValidForUser(string username, string password);
        bool storePerSecondStats(List<PerSecondStat> distinctStats);
        List<TestObject> testDatabase();
    }
}