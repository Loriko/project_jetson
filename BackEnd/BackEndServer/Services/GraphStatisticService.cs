using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.Enums;
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
            GraphStatistics graphStatistics = new GraphStatistics();
            List<string[]> maxStats = new List<string[]>();
            //using a placeholder time right now since we dont have data coming in at the same time. 
            //Ideally the end datetime should be the current datetime and the add seconds should be bigger for yearly
            DateTime end = new DateTime(2018, 4, 11, 14, 53, 18);
            DateTime start = end.AddSeconds(-5);

            for (int i = 0; i < 200; i++)
            {
                DatabaseGraphStat value = _databaseQueryService.getGraphStatByTimeInterval(cameraId, start, end);
                int epoch = (int)(value.End - new DateTime(1970, 1, 1)).TotalSeconds;
                maxStats.Add(new string[2] { epoch.ToString(), value.MaximumDetectedObjects.ToString() });
                start = start.AddSeconds(5);
                end = end.AddSeconds(5);
            }
            graphStatistics.Stats = maxStats.ToArray();

            return graphStatistics;
        }

        public GraphStatistics GetLast30MinutesStatistics(int cameraId)
        {
            return GetStatisticsForPastPeriod(cameraId, PastPeriod.LastHalfHour);
        }

        //Grab Graph Stats between an interval of time using unix time and specify the interval between each data point in seconds for a specific camera.
        //For example, between 1514808000 (01/01/2018 @ 12:00pm (UTC)) and 1517486400 (02/01/2018 @ 12:00pm (UTC)) with each point 3600 seconds apart (1 hour)
        public GraphStatistics GetGraphStatisticsByInterval(int cameraId,int startDate, int endDate,int interval)
        {
            GraphStatistics graphStatistics = new GraphStatistics();
            List<string[]> maxStats = new List<string[]>();

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime end = epoch.AddSeconds(endDate);
            DateTime start = end.AddSeconds(interval * -1);

            for (int i = endDate; i > startDate; i = i - interval)
            {
                DatabaseGraphStat value = _databaseQueryService.getGraphStatByTimeInterval(cameraId, start, end);
                int epochUnixTime = (int)(value.End - new DateTime(1970, 1, 1)).TotalSeconds;
                maxStats.Add(new string[2] { epochUnixTime.ToString(), value.MaximumDetectedObjects.ToString() });
                start = start.AddSeconds(5);
                end = end.AddSeconds(5);
            }
            // maxStats.Add(new string[2] { "dateTime", "People" });
            maxStats.Reverse();
            graphStatistics.Stats = maxStats.ToArray();

            return graphStatistics;
        }

        public GraphStatistics GetStatisticsForPastPeriod(int cameraId, PastPeriod pastPeriod, DateTime? startDate = null, DateTime? endDate = null)
        {
            GraphStatistics graphStatistics = new GraphStatistics();
            List<DatabasePerSecondStat> perSecondStats = _databaseQueryService.GetPerSecondStatsForCamera(cameraId);

            if (pastPeriod == PastPeriod.LastHalfHour)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddMinutes(-30) >= stat.DateTime);
            } else if (pastPeriod == PastPeriod.LastDay)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddDays(-1) >= stat.DateTime);
            } else if (pastPeriod == PastPeriod.LastWeek)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddDays(-7) >= stat.DateTime);
            } else if (pastPeriod == PastPeriod.LastMonth)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddMonths(-1) >= stat.DateTime);
            } else if (pastPeriod == PastPeriod.LastYear)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddYears(-1) >= stat.DateTime);
            } else if (pastPeriod == PastPeriod.Custom)
            {
                if (startDate == null || endDate == null)
                {
                    throw new InvalidDataException(
                        "Graph on custom date range requested but startDate or endDate is null");
                }
                perSecondStats.RemoveAll(stat => startDate.Value >= stat.DateTime || endDate.Value <= stat.DateTime);
                graphStatistics.StartDate = startDate;
                graphStatistics.EndDate = endDate;
            }

            List<string[]> perSecondStatsFormattedStrings = new List<string[]>();
            perSecondStatsFormattedStrings.Add(new [] { "DateTime", "People" });
            
            foreach (DatabasePerSecondStat perSecondStat in perSecondStats)
            {
                perSecondStatsFormattedStrings.Add(new []{perSecondStat.DateTime.ToUniversalTime().ToString("s"), perSecondStat.NumDetectedObjects.ToString()});
            }
            
            if (perSecondStats.Count == 0)
            {
                perSecondStatsFormattedStrings.Add(new []{DateTime.UtcNow.ToString("s"), 0.ToString()});
            }
            
            graphStatistics.Stats = perSecondStatsFormattedStrings.ToArray();
            graphStatistics.SelectedPeriod = pastPeriod;
            return graphStatistics;
        }
    }
}