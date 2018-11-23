using System;
using System.Collections.Generic;
using System.IO;
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

            // Obtain a list of all valid Camera Keys currently in the system.
            List<string> validCameraKeyList = _dbQueryService.GetAllCameraKeys();

            for (int z = 0; z < message.GetLength(); z++)
            {
                // Verify the attributes of the PerSecondStat object, except for the  CameraKey.
                if (message.RealTimeStats[z].isValidSecondStat() == false)
                {
                    return false;
                }

                // Verify its CameraKey.
                string cameraKeyToVerify = message.RealTimeStats[z].CameraKey;

                bool cameraKeyIsValid = validCameraKeyList.Where(o => string.Equals(cameraKeyToVerify, o, StringComparison.OrdinalIgnoreCase)).Any();

                if (cameraKeyIsValid == false)
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
                    if (String.IsNullOrWhiteSpace(message.RealTimeStats[c].CameraKey))
                    {
                        temp.Add("CameraKey");
                    }

                    // Does not query the camera keys again in order to avoid server slow down.

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
            List<DatabasePerSecondStat> dbSecondsToPersist = new List<DatabasePerSecondStat>();

            // Remove any possible duplicates.
            List<PerSecondStat> distinctPerSecondStats = verifiedMessage.RealTimeStats.Distinct().ToList();

            for (int y = 0; y < distinctPerSecondStats.Count; y++)
            {
                PerSecondStat stat = distinctPerSecondStats[y];

                int cameraId = _dbQueryService.GetCameraIdFromKey(stat.CameraKey);

                // Only persist PerSecondStats from valid Cameras (with valid CameraKeys).
                if (cameraId > 0)
                {
                    DatabasePerSecondStat dbPerSecondStat = new DatabasePerSecondStat();
                    dbPerSecondStat.CameraId = cameraId;
                    dbPerSecondStat.DateTime = MySqlDateTimeConverter.ToDateTime(stat.DateTime);
                    dbPerSecondStat.HasSavedImage = stat.HasSavedImage;
                    dbPerSecondStat.PerHourStatId = null;
                    dbPerSecondStat.NumDetectedObjects = stat.NumTrackedPeople;

                    // If PerSecondStat has a key frame.
                    if (stat.HasSavedImage && String.IsNullOrWhiteSpace(stat.FrameAsJpg) == false)
                    {
                        // Save it to the server.
                        dbPerSecondStat.FrameJpgPath = SaveKeyImage(stat);
                    }
                    else
                    {
                        dbPerSecondStat.HasSavedImage = false;
                        dbPerSecondStat.FrameJpgPath = null;
                    }

                    dbSecondsToPersist.Add(dbPerSecondStat);
                }
            }

            return _dbQueryService.PersistNewPerSecondStats(dbSecondsToPersist);
        }

        // Saves the key frame by converting the string attrubute to a Jpeg image file on the server and
        // Returns the file path to where it was saved.
        private static string SaveKeyImage(PerSecondStat stat)
        {
            string modifiedTimestamp = stat.DateTime.Substring(0, 19).Replace(" ", "").Replace(":", "").Replace("-", "");
            string folderPath = DatabasePerSecondStat.FRM_JPG_FOLDER_PATH + stat.CameraKey;
            DirectoryInfo outputDirectory = Directory.CreateDirectory(folderPath);
            string fullFilePath = Path.Combine(outputDirectory.FullName, "stat_frm" + modifiedTimestamp + ".jpg");
            bool success = ImageDecodingTools.SaveBase64StringToFile(stat.FrameAsJpg,
                fullFilePath);
            if (success)
            {
                return fullFilePath;
            }
            return null;
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

        /*
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
        */
    }
}