using System;
using System.Collections.Generic;
using System.Data;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Classes.ErrorResponseClasses;
using System.Linq;
using BackEndServer.Services.HelperServices;
using BackEndServer.Models.DBModels;
using Castle.Core.Internal;

namespace BackEndServer.Services
{
    public class DataMessageService : AbstractDataMessageService
    {
        // Attribute
        private readonly DatabaseQueryService _dbQueryService;

        // Constructor
        public DataMessageService(DatabaseQueryService dbQueryService)
        {
            this._dbQueryService = dbQueryService;
        }

        // Validates a received DataMessage object's contents (all PerSecondStat objects contained).
        public bool CheckDataMessageValidity(DataMessage message)
        {
            if (message == null || message.IsEmpty())
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

        // Creates an error response body custom to the characteristics of the received DataMessage object.
        public InvalidDataMessageResponseBody CreateInvalidDataMessageResponseBody(DataMessage message)
        {
            if (message == null || message.IsEmpty())
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
                    else if (message.RealTimeStats[c].DateTime.ToDateTime().ValidateDateTime() == false)
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

        // Once a DataMessage is verified, this method allows persisting all contents (PerSecondStat objects) into the database.
        public bool StoreStatsFromDataMessage(DataMessage verifiedMessage)
        {
            List<PerSecondStat> temp = new List<PerSecondStat>();

            for (int y = 0; y < verifiedMessage.GetLength(); y++)
            {
                int associatedCameraId = _dbQueryService.GetCameraIdFromKey(verifiedMessage.RealTimeStats[y].CameraKey);
                if (associatedCameraId != -1)
                {
                    verifiedMessage.RealTimeStats[y].CameraId = associatedCameraId;
                }
                else
                {
                    //If we can't get the camera id from the camera key, we won't know what to save the stat against,
                    //might as well drop it all 
                    return false;
                }

                temp.Add(verifiedMessage.RealTimeStats[y]);
            }

            // Remove any possible duplicates.
            List<PerSecondStat> distinctStats = temp.Distinct().ToList();

            foreach (PerSecondStat stat in distinctStats)
            {
                SaveStatJpgIfNecessary(stat);
            }

            return _dbQueryService.PersistNewPerSecondStats(distinctStats);
        }

        private static void SaveStatJpgIfNecessary(PerSecondStat stat)
        {
            string modifiedTimestamp = stat.DateTime.Replace(" ", "").Replace(":", "").Replace("-", "");
            string filePath = DatabasePerSecondStat.FRM_JPG_FOLDER_PATH + "stat_frm" + modifiedTimestamp + ".jpg";
            bool success = ImageDecodingTools.SaveBase64StringToFile(stat.FrameAsJpg,
                filePath);
            if (success)
            {
                stat.FrameAsJpgPath = filePath;
            }
        }


        // Before processing a request for a DataMessage with all PerSecondStat objects within a TimeInterval, this method is used to validate the received TimeInterval.
        public bool CheckTimeIntervalValidity(TimeInterval timeInterval)
        {
            if (timeInterval.StartDateTime.CheckIfSQLFormat() == false || timeInterval.EndDateTime.CheckIfSQLFormat() == false)
            {
                return false;
            }

            DateTime start = timeInterval.StartDateTime.ToDateTime();
            DateTime end = timeInterval.EndDateTime.ToDateTime();

            if (DateTimeTools.ValidateDateTime(start) == false || DateTimeTools.ValidateDateTime(end) == false)
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

        // Used for processing a request for a DataMessage with all PerSecondStat objects within a TimeInterval.
        public DataMessage RetrievePerSecondStatsBetweenInterval(TimeInterval verifiedTimeInterval)
        {
            List<DatabasePerSecondStat> queryResults = _dbQueryService.GetStatsFromInterval(verifiedTimeInterval);

            PerSecondStat[] stats = new PerSecondStat[queryResults.Count()];

            int x = 0;

            Dictionary<int, string> cameraKeyMap = new Dictionary<int, string>();
            
            foreach (DatabasePerSecondStat second in queryResults)
            {
                if (!cameraKeyMap.ContainsKey(second.CameraId))
                {
                    string possibleKey = _dbQueryService.GetCameraKeyFromId(second.CameraId);
                    if (possibleKey.IsNullOrEmpty())
                    {
                        throw new DataException("Database has a camera which doesn't have a key");
                    }

                    cameraKeyMap[second.CameraId] = possibleKey;
                }
                string cameraKey = cameraKeyMap[second.CameraId];

                PerSecondStat temp = new PerSecondStat(second.DateTime.ToString("yyyy-MM-dd HH:mm:ss"), cameraKey, second.NumDetectedObjects, second.HasSavedImage);
                stats[x] = temp;
                x++;
            }

            return new DataMessage("SYSTEM_RESPONSE", stats);
        }
    }
}