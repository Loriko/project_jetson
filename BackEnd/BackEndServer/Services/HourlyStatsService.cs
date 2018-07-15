using System;
using System.Collections.Generic;
using System.Linq;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;

namespace BackEndServer.Services
{
    public class HourlyStatsService : AbstractHourlyStatsService
    {
        private readonly IDatabaseQueryService _dbQueryService;

        // Constructor
        public HourlyStatsService(IDatabaseQueryService dbQueryService)
        {
            _dbQueryService = dbQueryService;
        }

        // Constructor for unit testing purposes.
        public HourlyStatsService()
        {
            this._dbQueryService = null;
        }

        public void AutoCalculateHourlyStats(DataMessage dataMessage)
        {
            // Obtain a list of all the unique Cameras sending data through the DataMessage. 
            List<int> cameraIdsToProcess = dataMessage.RealTimeStats
                                           .Select(x => x.CameraId).Distinct()
                                           .ToList<int>();

            foreach(int cameraId in cameraIdsToProcess)
            {
                // Obtain all of the camera's PerSecondStats within the DataMessage.
                List<PerSecondStat> cameraStats = dataMessage.RealTimeStats
                                                  .Where(x => x.CameraId == cameraId)
                                                  .ToList<PerSecondStat>();

                List<DateTime> hoursToCalculateStats = this.GetHoursToBeCalculated(cameraStats);

                if (hoursToCalculateStats != null)
                {
                    if (hoursToCalculateStats.Count == 0)
                    {
                        // There are no hours ready for calculation of hourly averages (min, max, average).
                        return;
                    }

                    // Calculate statistics and create the PerHourStats objects that will be stored in the database.
                    List<DatabasePerHourStat> perHourStats = CalculateHourlyAverages(hoursToCalculateStats, cameraId);

                    if (perHourStats == null)
                    {
                        // LOG error here and exit.
                        return;
                    }
                    else if (perHourStats.Count == 0)
                    {
                        // LOG error here and exit.
                        return;
                    }

                    bool successfulPersist = _dbQueryService.PersistNewPerHourStats(perHourStats);

                    if (successfulPersist == false)
                    {
                        // Write error to LOG
                    }
                }
            }
        }

        public List<DatabasePerHourStat> CalculateHourlyAverages(List<DateTime> hoursToCalulate, int cameraId)
        {
            List<DatabasePerHourStat> perHourStats = new List<DatabasePerHourStat>();

            foreach(DateTime hour in hoursToCalulate)
            {
                List<DatabasePerSecondStat>  tempAllSecondsInHourForCamera = _dbQueryService.GetAllSecondsForHourForCamera(hour, cameraId);

                if (tempAllSecondsInHourForCamera.Count != 3600)
                {
                    // Write to LOG that this hour is missing PerSecondStats.
                    return null; 
                }

                DatabasePerHourStat hourStat = null;
                int minPeopleInHour = tempAllSecondsInHourForCamera.First().NumDetectedObjects;
                int maxPeopleInHour = tempAllSecondsInHourForCamera.First().NumDetectedObjects;
                int tempSum = 0;

                // Calculate average, min and max. Create DatabasePerHourStat.
                foreach (DatabasePerSecondStat second in tempAllSecondsInHourForCamera)
                {
                    tempSum += second.NumDetectedObjects;

                    if (second.NumDetectedObjects < minPeopleInHour)
                    {
                        minPeopleInHour = second.NumDetectedObjects;
                    }

                    if (second.NumDetectedObjects > maxPeopleInHour)
                    {
                        maxPeopleInHour = second.NumDetectedObjects;
                    }
                }

                hourStat = new DatabasePerHourStat
                {
                    CameraId = cameraId,
                    Day = DateTimeTools.GetHourBeginning(hour),
                    Hour = hour.Hour,
                    AverageDetectedObjects = (tempSum / tempAllSecondsInHourForCamera.Count),
                    MaximumDetectedObjects = maxPeopleInHour,
                    MinimumDetectedObjects = minPeopleInHour
                };

                perHourStats.Add(hourStat);     
            }

            return perHourStats;
        }

        public List<DateTime> GetHoursToBeCalculated(List<PerSecondStat> uniqueCameraStats)
        {
            List<DateTime> hoursToCalculateAverages = new List<DateTime>();

            foreach (PerSecondStat second in uniqueCameraStats)
            {
                // Create this function in the Helper Sevices DateTimeTools class.
                // Regular Expression that checks if the PerSecondStat is the last second of that hour.
                if (MySqlDateTimeTools.IsLastSecondOfHour(second.DateTime))
                {
                    hoursToCalculateAverages.Add(MySqlDateTimeConverter.ToDateTime(second.DateTime).GetHourBeginning());
                }
            }

            return hoursToCalculateAverages;
        }
    }
}