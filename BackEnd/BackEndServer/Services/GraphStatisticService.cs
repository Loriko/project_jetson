using System;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.PlaceholderServices;

namespace BackEndServer.Services
{
    public class GraphStatisticService : AbstractGraphStatisticService
    {
        private readonly IDatabaseQueryService _databaseQueryService;

        public GraphStatisticService(IDatabaseQueryService databaseQueryService)
        {
            _databaseQueryService = databaseQueryService;
        }

        public GraphStatistics GetYearlyGraphStatistics(int cameraId)
        {
            throw new System.NotImplementedException();
        }

        public GraphStatistics GetLast30MinutesStatistics(int cameraId)
        {
            GraphStatistics graphStatistics = new GraphStatistics();
            List<DatabasePerSecondStat> perSecondStats = _databaseQueryService.GetPerSecondStatsForCamera(cameraId);
            
            perSecondStats.RemoveAll(stat => DateTime.Now.AddMinutes(-30.0) >= stat.DateTime);
            
            if (perSecondStats.Count == 0)
            {
                return null;
            }
            
            List<string[]> perSecondStatsFormattedStrings = new List<string[]>();
            perSecondStatsFormattedStrings.Add(new string[2] { "Time", "People" });
            
            foreach (DatabasePerSecondStat perSecondStat in perSecondStats)
            {
                perSecondStatsFormattedStrings.Add(new string[2]{perSecondStat.DateTime.ToString("HH:mm:ss"), perSecondStat.NumDetectedObjects.ToString()});
            }
            graphStatistics.Stats = perSecondStatsFormattedStrings.ToArray();
            return graphStatistics;
        }
    }
}