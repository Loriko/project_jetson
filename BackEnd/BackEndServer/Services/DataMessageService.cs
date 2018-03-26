﻿using System;
using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Classes.ErrorResponseClasses;
using System.Linq;
using BackEndServer.Services.HelperServices;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Services
{
    public class DataMessageService : AbstractDataMessageService
    {
        private readonly DatabaseQueryService _dbQueryService = new DatabaseQueryService();

        public bool checkDataMessageValidity(DataMessage message)
        {
            if (message.IsEmpty())
            {
                return false;
            }

            for (int z = 0; z < message.GetLength(); z++)
            {
                if (message.RealTimeStats[z].isValidSecondStat() == false)
                {
                    return false;
                }
            }

            return true;
        }

        public InvalidDataMessageResponseBody createInvalidDataMessageResponseBody(DataMessage message)
        {
            if (message.IsEmpty())
            {
                return new InvalidDataMessageResponseBody(true, null);
            }
            else
            {
                List<string> temp = new List<string>();

                for (int c = 0; c < message.GetLength(); c++)
                {
                    if (message.RealTimeStats[c].CameraId < 0)
                    {
                        temp.Add("CameraId");
                    }

                    if (message.RealTimeStats[c].NumTrackedPeople < 0)
                    {
                        temp.Add("NumTrackedPeople");
                    }

                    if (MySqlDateTimeConverter.CheckIfSQLFormat(message.RealTimeStats[c].DateTime) == false)
                    {
                        temp.Add("DateTime");
                    }
                    else if (message.RealTimeStats[c].DateTime.toDateTime().validateDateTime() == false)
                    {
                        temp.Add("DateTime");
                    }
                }

                // Removes all duplicates from the list of failed attributes.
                temp = temp.Distinct().ToList<string>();

                // Counter for next step.
                int x = 0;
                string[] failedAttributes = new string[temp.Count];

                // Store values in an array of strings.
                foreach (string distinctAttribute in temp)
                {
                    failedAttributes[x] = distinctAttribute;
                    x++;
                }

                return new InvalidDataMessageResponseBody(false, failedAttributes);
            }
        }

        public bool storeStatsFromDataMessage(DataMessage verifiedMessage)
        {
            List<PerSecondStat> temp = new List<PerSecondStat>();

            for (int y = 0; y < verifiedMessage.GetLength(); y++)
            {
                temp.Add(verifiedMessage.RealTimeStats[y]);
            }

            // Remove any possible duplicates.
            List<PerSecondStat> distinctStats = temp.Distinct().ToList();

            return this._dbQueryService.storePerSecondStats(distinctStats);
        }

        public bool checkTimeIntervalValidity(TimeInterval timeInterval)
        {
            if (timeInterval.StartDateTime.CheckIfSQLFormat() == false || timeInterval.EndDateTime.CheckIfSQLFormat() == false)
            {
                return false;
            }

            DateTime start = timeInterval.StartDateTime.toDateTime();
            DateTime end = timeInterval.EndDateTime.toDateTime();

            if (DateTimeTools.validateDateTime(start) == false || DateTimeTools.validateDateTime(end) == false)
            {
                return false;
            }

            // The start time must be before the end time or the start time must be identical to the end time.
            if (DateTime.Compare(start, end) > 0)
            {
                return false;
            }

            return true;
        }

        public DataMessage retrievePerSecondStatsBetweenInterval(TimeInterval verifiedTimeInterval)
        {
            List<DatabasePerSecondStat> queryResults = _dbQueryService.getStatsFromInterval(verifiedTimeInterval);

            PerSecondStat[] stats = new PerSecondStat[queryResults.Count()];

            int x = 0;

            foreach (DatabasePerSecondStat second in queryResults)
            {
                PerSecondStat temp = new PerSecondStat(second.DateTime, second.CameraId, second.NumDetectedObjects, second.HasSavedImage);
                stats[x] = temp;
                x++;
            }

            return new DataMessage(stats);
        }
    }
}