using System;
using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Classes.ErrorResponseClasses;
using System.Linq;
using BackEndServer.Services.HelperServices;

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

        public bool storeStatsFromDataMessage(DataMessage message)
        {
            List<PerSecondStat> temp = new List<PerSecondStat>();

            for (int y = 0; y < message.GetLength(); y++)
            {
                temp.Add(message.RealTimeStats[y]);
            }

            // Remove any possible duplicates.
            List<PerSecondStat> distinctStats = temp.Distinct().ToList();

            return this._dbQueryService.storePerSecondStat(distinctStats);
        }
    }
}