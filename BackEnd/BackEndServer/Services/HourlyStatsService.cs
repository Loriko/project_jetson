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
            List<DateTime> hoursToCalculateAverages = this.CheckIfCalculationsRequired(dataMessage);

            if (hoursToCalculateAverages.Count == 0)
            {
                // There are no hours ready for calculation of hourly averages (min, max, average).
                return;
            }

            List<DatabasePerHourStat> perHourStats = CalculateHourlyAverages(hoursToCalculateAverages);

            if (perHourStats == null)
            {
                // Log error here and exit.
                return;
            }
            else if (perHourStats.Count == 0)
            {
                // Log error here and exit.
                return;
            }

            bool successfulPersist = _dbQueryService.PersistNewPerHourStats(perHourStats);

            if (successfulPersist == false)
            {
                // Write error to LOG
            }
        }

        public List<DatabasePerHourStat> CalculateHourlyAverages(List<DateTime> hoursToCalulate)
        {
            List<DatabasePerHourStat> perHourStats = new List<DatabasePerHourStat>();

            DatabasePerHourStat hourStat = null;
            List<DatabasePerSecondStat> tempAllSecondsInHour = null;
            int minPeopleInHour = -1;
            int maxPeopleInHour = -1;
            int tempSum = 0;

            foreach(DateTime hour in hoursToCalulate)
            {
                tempAllSecondsInHour = _dbQueryService.GetAllSecondsForHour(hour);

                if (tempAllSecondsInHour.Count != 3600)
                {
                    // Write to log that this hour is missing PerSecondStats.
                    return null; 
                }

                minPeopleInHour = tempAllSecondsInHour.First().NumDetectedObjects;
                maxPeopleInHour = tempAllSecondsInHour.First().NumDetectedObjects;
                tempSum = 0;

                // Calculate average, min and max. Create DatabasePerHourStat.
                foreach (DatabasePerSecondStat second in tempAllSecondsInHour)
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
                    Day = DateTimeTools.GetHourBeginning(hour),
                    Hour = hour.Hour,
                    AverageDetectedObjects = (tempSum / tempAllSecondsInHour.Count),
                    MaximumDetectedObjects = maxPeopleInHour,
                    MinimumDetectedObjects = minPeopleInHour
                };

                perHourStats.Add(hourStat);     
            }

            return perHourStats;
        }

        public List<DateTime> CheckIfCalculationsRequired(DataMessage dataMessage)
        {
            List<DateTime> hoursToCalculateAverages = new List<DateTime>();

            foreach(PerSecondStat second in dataMessage.RealTimeStats)
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