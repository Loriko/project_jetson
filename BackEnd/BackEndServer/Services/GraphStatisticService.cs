using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.Enums;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;

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
                start = start.AddSeconds(interval);
                end = end.AddSeconds(interval);
            }
            // maxStats.Add(new string[2] { "dateTime", "People" });
            maxStats.Reverse();
            graphStatistics.Stats = maxStats.ToArray();

            return graphStatistics;
        }

        public GraphStatistics GetStatisticsForPastPeriod(int cameraId, PastPeriod pastPeriod, DateTime? startDate = null, DateTime? endDate = null)
        {
            GraphStatistics graphStatistics = new GraphStatistics();
            if (startDate != null && endDate != null)
            {
                graphStatistics.StartDate = startDate;
                graphStatistics.EndDate = endDate;
            }

            List<DatabasePerSecondStat> perSecondStats = GetDatabasePerSecondStatsForPastPeriod(cameraId, pastPeriod, startDate, endDate);

            AddDelimiterStatsForPastPeriod(perSecondStats, pastPeriod, startDate, endDate);
            AddDummyStatsForDeadPeriods(perSecondStats);
            
            List<string[]> perSecondStatsFormattedStrings = new List<string[]>();
            perSecondStatsFormattedStrings.Add(new [] { "DateTime", "People Detected" });
            
            foreach (DatabasePerSecondStat perSecondStat in perSecondStats)
            {
                perSecondStatsFormattedStrings.Add(new []{DateTime.SpecifyKind(perSecondStat.DateTime, DateTimeKind.Local).ToString("o"), perSecondStat.NumDetectedObjects.ToString()});
            }
            
            graphStatistics.Stats = perSecondStatsFormattedStrings.ToArray();
            graphStatistics.SelectedPeriod = pastPeriod;
            return graphStatistics;
        }

        private void AddDummyStatsForDeadPeriods(List<DatabasePerSecondStat> perSecondStats)
        {
            List<DatabasePerSecondStat> dummyStats = new List<DatabasePerSecondStat>();
            for (int i = 0; i < perSecondStats.Count - 1; i++)
            {
                DatabasePerSecondStat previousStat = perSecondStats[i];
                DatabasePerSecondStat nextStat = perSecondStats[i + 1];
                if (nextStat.DateTime.Subtract(previousStat.DateTime).Minutes > 2)
                {
                    dummyStats.Add(new DatabasePerSecondStat
                    {
                        DateTime = previousStat.DateTime.AddSeconds(5),
                        NumDetectedObjects = 0
                    });
                    dummyStats.Add(new DatabasePerSecondStat
                    {
                        DateTime = nextStat.DateTime.AddSeconds(-5),
                        NumDetectedObjects = 0
                    });
                }
            }
            perSecondStats.AddRange(dummyStats);
        }

        public GraphStatistics GetSharedRoomStatisticsForPastPeriod(int roomId, PastPeriod pastPeriod, DateTime? startDate = null,
            DateTime? endDate = null)
        {
            List<DatabaseCamera> dbCameras = _databaseQueryService.GetAllCamerasInRoom(roomId);
            List<int> cameraIds = dbCameras.Select(camera => camera.CameraId).ToList();
            return GetSharedRoomStatisticsForPastPeriod(cameraIds, pastPeriod, startDate, endDate);
        }

        public GraphStatistics GetSharedRoomStatisticsForPastPeriod(List<int> cameraIds, PastPeriod pastPeriod, DateTime? startDate = null, DateTime? endDate = null)
        {
            GraphStatistics graphStatistics = new GraphStatistics();
            if (startDate != null && endDate != null)
            {
                graphStatistics.StartDate = startDate;
                graphStatistics.EndDate = endDate;
            }

            List<List<DatabasePerSecondStat>> perSecondStatsForEachCamera = new List<List<DatabasePerSecondStat>>();

            foreach (var cameraId in cameraIds)
            {
                perSecondStatsForEachCamera.Add(GetDatabasePerSecondStatsForPastPeriod(cameraId, pastPeriod, startDate, endDate));
            }

            foreach (var perSecondStats in perSecondStatsForEachCamera)
            {
                AddDelimiterStatsForPastPeriod(perSecondStats, pastPeriod, startDate, endDate);
                AddDummyStatsForDeadPeriods(perSecondStats);
            }
            
            List<string[]> perSecondStatsFormattedStrings = new List<string[]>();
            string[] titleString = new string[perSecondStatsForEachCamera.Count+1];
            titleString[0] = "DateTime";
            for (int i = 0; i < cameraIds.Count; i++)
            {
                int cameraId = cameraIds.ElementAt(i);
                DatabaseCamera camera = _databaseQueryService.GetCameraById(cameraId);
                titleString[i+1] = $"People Detected for {camera.CameraName}";
            }
            perSecondStatsFormattedStrings.Add(titleString);

            for (int i = 1; i <= perSecondStatsForEachCamera.Count; i++)
            {
                List<DatabasePerSecondStat> perSecondStats = perSecondStatsForEachCamera.ElementAt(i-1);
                foreach (DatabasePerSecondStat perSecondStat in perSecondStats)
                {
                    string[] row = new string[perSecondStatsForEachCamera.Count+1];
                    row[0] = perSecondStat.DateTime.ToString("o");
                    row[i] = perSecondStat.NumDetectedObjects.ToString();
                    perSecondStatsFormattedStrings.Add(row);
                }
            }
            
            graphStatistics.Stats = perSecondStatsFormattedStrings.ToArray();
            graphStatistics.SelectedPeriod = pastPeriod;
            return graphStatistics;
        }

        private void AddDelimiterStatsForPastPeriod(List<DatabasePerSecondStat> perSecondStats, PastPeriod pastPeriod, 
                                                    DateTime? startDate, DateTime? endDate = null)
        {
            perSecondStats.Insert(0, new DatabasePerSecondStat
            {
                DateTime = GetLatestDateForPastPeriod(pastPeriod, startDate),
                NumDetectedObjects = 0
            });
            perSecondStats.Add(new DatabasePerSecondStat
            {
                DateTime = pastPeriod == PastPeriod.Custom && endDate != null ? 
                    endDate.Value : DateTime.Now,
                NumDetectedObjects = 0
            });
        }

        private DateTime GetLatestDateForPastPeriod(PastPeriod pastPeriod, DateTime? startDate = null)
        {
            if (pastPeriod == PastPeriod.LastHalfHour)
            {
                return DateTime.Now.AddMinutes(-30);
            }
            else if (pastPeriod == PastPeriod.LastDay)
            {
                return DateTime.Now.AddDays(-1);
            }
            else if (pastPeriod == PastPeriod.LastWeek)
            {
                return DateTime.Now.AddDays(-7);
            }
            else if (pastPeriod == PastPeriod.LastMonth)
            {
                return DateTime.Now.AddMonths(-1);
            }
            else if (pastPeriod == PastPeriod.LastYear)
            {
                return DateTime.Now.AddYears(-1);
            }
            else if (pastPeriod == PastPeriod.Custom && startDate != null)
            {
                return startDate.Value;
            }

            throw new ArgumentException("Can't find Latest Valid Date for specified arguments");
        }
        
        private List<DatabasePerSecondStat> GetDatabasePerSecondStatsForPastPeriod(int cameraId, PastPeriod pastPeriod, 
            DateTime? startDate, DateTime? endDate)
        {
            List<DatabasePerSecondStat> perSecondStats = _databaseQueryService.GetPerSecondStatsForCamera(cameraId);
            //Remove all stats set into the future
            perSecondStats.RemoveAll(stat => DateTime.Now < stat.DateTime);
            if (pastPeriod == PastPeriod.LastHalfHour)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddMinutes(-30) >= stat.DateTime);
            }
            else if (pastPeriod == PastPeriod.LastDay)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddDays(-1) >= stat.DateTime);
            }
            else if (pastPeriod == PastPeriod.LastWeek)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddDays(-7) >= stat.DateTime);
            }
            else if (pastPeriod == PastPeriod.LastMonth)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddMonths(-1) >= stat.DateTime);
            }
            else if (pastPeriod == PastPeriod.LastYear)
            {
                perSecondStats.RemoveAll(stat => DateTime.Now.AddYears(-1) >= stat.DateTime);
            }
            else if (pastPeriod == PastPeriod.Custom)
            {
                if (startDate == null || endDate == null)
                {
                    throw new InvalidDataException(
                        "Graph on custom date range requested but startDate or endDate is null");
                }
                perSecondStats.RemoveAll(stat => startDate.Value >= stat.DateTime || endDate.Value <= stat.DateTime);
            }

            return perSecondStats;
        }
    }
}