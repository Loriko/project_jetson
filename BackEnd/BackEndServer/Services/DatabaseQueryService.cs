using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using BackEndServer.Models.DBModels;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using BackEndServer.Models.APIModels;
using System.ComponentModel;
using BackEndServer.Models.Enums;
using Castle.Core.Internal;
using static BackEndServer.Models.Enums.TriggerOperatorExtension;

// More Info: http://www.c-sharpcorner.com/article/how-to-connect-mysql-with-asp-net-core/

// Bulk insert: "INSERT INTO tbl_name (a,b,c) VALUES(1,2,3),(4,5,6),(7,8,9)"

namespace BackEndServer.Services
{   
    public class DatabaseQueryService : IDatabaseQueryService
    {
        private IDatabaseQueryService _databaseQueryServiceImplementation;

        #region Database Context

        // Connection String Attribute
        public string ConnectionString { get; set; }

        // Constructor
        public DatabaseQueryService(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        
        // Method to return a MySQL Database connection.
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        #endregion

        #region Data Receival Controller (API)

        public bool PersistNewPerSecondStats(List<PerSecondStat> distinctStats)
        {
            // Define the bulk insert query without any values to insert.
            string bulkInsertCommand = $"INSERT INTO {DatabasePerSecondStat.TABLE_NAME} "
                + $"({DatabasePerSecondStat.CAMERA_ID_LABEL},{DatabasePerSecondStat.NUM_DETECTED_OBJECTS_LABEL}, "
                + $"{DatabasePerSecondStat.DATE_TIME_LABEL},{DatabasePerSecondStat.HAS_SAVED_IMAGE_LABEL},{DatabasePerSecondStat.FRM_JPG_PATH_LABEL}) VALUES ";

            // Append the values one by one to the bulk insert query.
            PerSecondStat lastStat = distinctStats.Last();

            foreach (PerSecondStat stat in distinctStats)
            {
                //TODO: Usafe addition done for milestone 4
                string cameraId = stat.CameraId.Value.ToString();
                string numDetectedObjects = stat.NumTrackedPeople.ToString();
                string hasImage = "0";

                if (stat.HasSavedImage)
                {
                    hasImage = "1";
                }

                bulkInsertCommand += $"({cameraId},{numDetectedObjects},'{stat.DateTime}',{hasImage},{formatNullableString(stat.FrameAsJpgPath)})";

                if (stat != lastStat)
                {
                    bulkInsertCommand += ",";
                }
            }

            // Open connection and execute bulk insert command.
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                try
                {
                    MySqlCommand cmd = new MySqlCommand(bulkInsertCommand, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Write to Log
                    return false; // This is probably not going to be executed...
                }
            }
            return true;
        }

        #endregion

        #region Hourly Stats Service (API)

        public bool PersistNewPerHourStats(List<DatabasePerHourStat> perHourStats)
        {
            // Define the bulk insert query without any values to insert.
            string bulkInsertCommand = $"INSERT INTO {DatabasePerHourStat.TABLE_NAME} "
                + $"({DatabasePerHourStat.DATE_DAY_LABEL},{DatabasePerHourStat.DATE_HOUR_LABEL},{DatabasePerHourStat.MAX_DETECTED_OBJECT_LABEL},"
                + $"{DatabasePerHourStat.MIN_DETECTED_OBJECT_LABEL},{DatabasePerHourStat.AVG_DETECTED_OBJECT_LABEL}) VALUES ";

            // Append the values one by one to the bulk insert query.
            DatabasePerHourStat lastHourStat = perHourStats.Last();

            foreach (DatabasePerHourStat hourStat in perHourStats)
            {
                string dateDay = MySqlDateTimeConverter.ToMySqlDateString(hourStat.Day);
                string dateHour = hourStat.Hour.ToString();
                string hourMax = hourStat.MaximumDetectedObjects.ToString();
                string hourMin = hourStat.MinimumDetectedObjects.ToString();
                string hourAverage = hourStat.AverageDetectedObjects.ToString();

                bulkInsertCommand += $"('{dateDay}',{dateHour},{hourMax},{hourMin},{hourAverage})";

                if (hourStat != lastHourStat)
                {
                    bulkInsertCommand += ",";
                }
            }

            // Open connection and execute bulk insert command.
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                try
                {
                    MySqlCommand cmd = new MySqlCommand(bulkInsertCommand, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Write to Log
                    return false; // This is probably not going to be executed...
                }
            }
            return true;
        }

        public List<DatabasePerSecondStat> GetAllSecondsForHourForCamera(DateTime dateTime, int cameraId)
        {
            string startDateTime = dateTime.GetHourBeginning().ToMySqlDateTimeString();
            string endDateTime = dateTime.GetHourEnd().ToMySqlDateTimeString();

            List<DatabasePerSecondStat> perSecondStatsList = new List<DatabasePerSecondStat>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabasePerSecondStat.TABLE_NAME} "
                    + $"WHERE {DatabasePerSecondStat.DATE_TIME_LABEL} >= '{startDateTime}' "
                    + $"AND {DatabasePerSecondStat.DATE_TIME_LABEL} <= '{endDateTime}' "
                    + $"AND {DatabasePerSecondStat.CAMERA_ID_LABEL} = {cameraId}";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStatsList.Add(new DatabasePerSecondStat()
                        {
                            PerSecondStatId = Convert.ToInt32(reader[DatabasePerSecondStat.PER_SECOND_STAT_ID_LABEL]),
                            DateTime = Convert.ToDateTime(reader[DatabasePerSecondStat.DATE_TIME_LABEL]),
                            CameraId = Convert.ToInt32(reader[DatabasePerSecondStat.CAMERA_ID_LABEL]),
                            NumDetectedObjects = Convert.ToInt32(reader[DatabasePerSecondStat.NUM_DETECTED_OBJECTS_LABEL]),
                            HasSavedImage = Convert.ToBoolean(Convert.ToInt16(reader[DatabasePerSecondStat.HAS_SAVED_IMAGE_LABEL])),
                            // Null by default, this is what will be updated later by the HourlyStatsService.
                            PerHourStatId = -1
                        });
                    }
                }
            }
            return perSecondStatsList;
        }

        public DatabasePerHourStat GetPerHourStatFromHour(DateTime hour)
        {
            DatabasePerHourStat perHourStat = null;

            string mySqlDateString = hour.ToMySqlDateString();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabasePerHourStat.TABLE_NAME} "
                    + $"WHERE {DatabasePerHourStat.DATE_DAY_LABEL} = '{mySqlDateString}' " +
                    $"AND {DatabasePerHourStat.DATE_HOUR_LABEL} = {hour.Hour} LIMIT 1";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        perHourStat = new DatabasePerHourStat()
                        {
                            PerHourStatId = Convert.ToInt32(reader[DatabasePerHourStat.PER_HOUR_STAT_ID_LABEL]),
                            Day = MySqlDateTimeConverter.ToDateTime(Convert.ToString(reader[DatabasePerHourStat.DATE_DAY_LABEL])),
                            Hour = Convert.ToInt32(reader[DatabasePerHourStat.DATE_HOUR_LABEL]),
                            AverageDetectedObjects = Convert.ToDouble(reader[DatabasePerHourStat.AVG_DETECTED_OBJECT_LABEL]),
                            MaximumDetectedObjects = Convert.ToInt32(reader[DatabasePerHourStat.MAX_DETECTED_OBJECT_LABEL]),
                            MinimumDetectedObjects = Convert.ToInt32(reader[DatabasePerHourStat.MIN_DETECTED_OBJECT_LABEL])
                        };
                    }
                }
            }
            return perHourStat;
        }

        public bool UpdatePerSecondStatsWithPerHourStatId(DateTime hour, int perHourStatId)
        {
            string startDateTime = hour.GetHourBeginning().ToMySqlDateTimeString();
            string endDateTime = hour.GetHourEnd().ToMySqlDateTimeString();

            // Define the update command with the values to update.
            string updateCommand = $"UPDATE {DatabasePerSecondStat.TABLE_NAME} "
                + $"SET {DatabasePerSecondStat.PER_HOUR_STAT_ID_LABEL} = {perHourStatId} "
                + $"WHERE {DatabasePerSecondStat.DATE_TIME_LABEL} >= '{startDateTime}' "
                + $"AND {DatabasePerSecondStat.DATE_TIME_LABEL} <= '{endDateTime}'";

            // Open connection and execute the update command.
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                try
                {
                    MySqlCommand cmd = new MySqlCommand(updateCommand, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Write to LOG
                    return false; // This is probably not going to be executed...
                }
            }
            return true;
        }

        #endregion

        // FRANCIS TO CHECK LATER 
        // Right now, username isn't used, but it will be as soon as the tables necessary for camera permissions are added.
        public List<DatabaseLocation> GetLocationsForUser(int userId)
        {
            List<DatabaseLocation> locationList = new List<DatabaseLocation>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseLocation.TABLE_NAME} " +
                               $"WHERE {DatabaseLocation.LOCATION_ID_LABEL} IN (" +
                                   $"SELECT {DatabaseCamera.LOCATION_ID_LABEL} FROM {DatabaseCamera.TABLE_NAME} " +
                                   $"WHERE {DatabaseCamera.CAMERA_ID_LABEL} IN ( " +
                                       "SELECT user_camera_association.camera_id " +
                                       "FROM user_camera_association " +
                                       $"WHERE user_camera_association.user_id = {userId} " +
                                   $") OR {DatabaseCamera.USER_ID_LABEL} = {userId}" +
                               ");";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        locationList.Add(new DatabaseLocation()
                        {
                            LocationId = Convert.ToInt32(reader[DatabaseLocation.LOCATION_ID_LABEL]),
                            LocationName = Convert.ToString(reader[DatabaseLocation.LOCATION_NAME_LABEL]),
                            AddressLine = Convert.ToString(reader[DatabaseLocation.ADDRESS_LINE_LABEL]),
                            City = Convert.ToString(reader[DatabaseLocation.CITY_LABEL]),
                            State = Convert.ToString(reader[DatabaseLocation.STATE_LABEL]),
                            Zip = Convert.ToString(reader[DatabaseLocation.ZIP_LABEL])
                        });
                    }
                }
            }
            return locationList;
        }

        public List<DatabasePerSecondStat> GetStatsFromInterval(TimeInterval verifiedTimeInterval)
        {
            List<DatabasePerSecondStat> perSecondStatsList = new List<DatabasePerSecondStat>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabasePerSecondStat.TABLE_NAME} "
                    + $"WHERE {DatabasePerSecondStat.DATE_TIME_LABEL} >= '{verifiedTimeInterval.StartDateTime}' "
                    + $"AND {DatabasePerSecondStat.DATE_TIME_LABEL} <= '{verifiedTimeInterval.EndDateTime}'";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStatsList.Add(new DatabasePerSecondStat()
                        {
                            PerSecondStatId = Convert.ToInt32(reader[DatabasePerSecondStat.PER_SECOND_STAT_ID_LABEL]),
                            DateTime = Convert.ToDateTime(reader[DatabasePerSecondStat.DATE_TIME_LABEL]),
                            CameraId = Convert.ToInt32(reader[DatabasePerSecondStat.CAMERA_ID_LABEL]),
                            NumDetectedObjects = Convert.ToInt32(reader[DatabasePerSecondStat.NUM_DETECTED_OBJECTS_LABEL]),
                            HasSavedImage = Convert.ToBoolean(Convert.ToInt16(reader[DatabasePerSecondStat.HAS_SAVED_IMAGE_LABEL]))
                        });
                    }
                }
            }
            return perSecondStatsList;
        }

        public DatabasePerSecondStat GetLatestPerSecondStatForCamera(int cameraId)
        {
            DatabasePerSecondStat perSecondStat = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabasePerSecondStat.TABLE_NAME} "
                    + $"WHERE {DatabasePerSecondStat.CAMERA_ID_LABEL} = {cameraId} "
                    + $"ORDER BY {DatabasePerSecondStat.DATE_TIME_LABEL} DESC LIMIT 1";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting only one row
                    if (reader.Read())
                    {
                        perSecondStat = new DatabasePerSecondStat()
                        {
                            PerSecondStatId = Convert.ToInt32(reader[DatabasePerSecondStat.PER_SECOND_STAT_ID_LABEL]),
                            DateTime = Convert.ToDateTime(reader[DatabasePerSecondStat.DATE_TIME_LABEL]),
                            CameraId = Convert.ToInt32(reader[DatabasePerSecondStat.CAMERA_ID_LABEL]),
                            NumDetectedObjects = Convert.ToInt32(reader[DatabasePerSecondStat.NUM_DETECTED_OBJECTS_LABEL]),
                            HasSavedImage = Convert.ToBoolean(Convert.ToInt16(reader[DatabasePerSecondStat.HAS_SAVED_IMAGE_LABEL]))
                        };
                    }
                }
            }
            return perSecondStat;
        }

        public int GetCameraIdFromKey(string cameraKey)
        {
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT {DatabaseCamera.CAMERA_ID_LABEL} FROM {DatabaseCamera.TABLE_NAME} "
                    + $"WHERE {DatabaseCamera.CAMERA_KEY_LABEL} = '{cameraKey}' LIMIT 1";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader[DatabaseCamera.CAMERA_ID_LABEL]);
                    }
                }
            }

            return -1;
        }
        
        public int GetAPIKeyIdFromKey(string apiKey)
        {
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT {DatabaseAPIKey.API_KEY_ID_LABEL} FROM {DatabaseAPIKey.TABLE_NAME} "
                               + $"WHERE {DatabaseAPIKey.API_KEY_LABEL} = '{apiKey}' LIMIT 1;";

                Console.WriteLine("\n");
                Console.WriteLine(query);
                Console.WriteLine("\n");
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader[DatabaseAPIKey.API_KEY_ID_LABEL]);
                    }
                }
            }

            return -1;
        }

        public DatabaseCamera GetCameraById(int cameraId)
        {
            DatabaseCamera camera = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseCamera.TABLE_NAME} WHERE {DatabaseCamera.CAMERA_ID_LABEL} = {cameraId}";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        camera = getDatabaseCameraFromReader(reader);
                    }
                }
            }

            return camera;
        }
        
        public DatabaseCamera GetCameraByKey(string cameraKey)
        {
            DatabaseCamera camera = null;
            
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseCamera.TABLE_NAME} WHERE {DatabaseCamera.CAMERA_KEY_LABEL} = '{cameraKey}'";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        camera = getDatabaseCameraFromReader(reader);
                    }
                }
            }

            return camera;
        }
        
        // FRANCIS, Not sure what this is for ??? Is there no cap ???
        public List<DatabasePerSecondStat> GetPerSecondStatsForCamera(int cameraId)
        {
            List<DatabasePerSecondStat> perSecondStats = new List<DatabasePerSecondStat>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabasePerSecondStat.TABLE_NAME} WHERE {DatabasePerSecondStat.CAMERA_ID_LABEL} = {cameraId}";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStats.Add(new DatabasePerSecondStat()
                        {
                            PerSecondStatId = Convert.ToInt32(reader[DatabasePerSecondStat.PER_SECOND_STAT_ID_LABEL]),
                            CameraId = Convert.ToInt32(reader[DatabasePerSecondStat.CAMERA_ID_LABEL]),
                            DateTime = Convert.ToDateTime(reader[DatabasePerSecondStat.DATE_TIME_LABEL]),
                            NumDetectedObjects =  Convert.ToInt32(reader[DatabasePerSecondStat.NUM_DETECTED_OBJECTS_LABEL]),
                            HasSavedImage = Convert.ToBoolean(reader[DatabasePerSecondStat.HAS_SAVED_IMAGE_LABEL]),
                            FrameJpgPath = Convert.ToString(reader[DatabasePerSecondStat.FRM_JPG_PATH_LABEL])
                        });
                    }
                }
            }
            return perSecondStats;
        }
        
        public List<DatabaseCamera> GetCamerasForLocation(int locationId)
        {
            List<DatabaseCamera> cameraList = new List<DatabaseCamera>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseCamera.TABLE_NAME} WHERE {DatabaseCamera.LOCATION_ID_LABEL} = {locationId}";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseCamera camera = getDatabaseCameraFromReader(reader);
                        cameraList.Add(camera);
                    }
                }
            }
            return cameraList;
        }
        
        public bool IsPasswordValidForUser(string username, string password)
        {
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseUser.TABLE_NAME} " +
                               $"WHERE {DatabaseUser.USERNAME_LABEL} = '{username}' " +
                               $"AND {DatabaseUser.PASSWORD_LABEL} = '{password}' LIMIT 1";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read() && Convert.ToString(reader["username"]) == username)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public int? GetUserIdByUsername(string username)
        {
            int? idToReturn = null;
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT id FROM {DatabaseUser.TABLE_NAME} WHERE username = '{username}';";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    List<int?> ids = new List<int?>();
                    while (reader.Read())
                    {
                        ids.Add(Convert.ToInt32(reader["id"]));
                    }

                    if (ids.Count == 1)
                    {
                        idToReturn = ids[0];
                    }
                    else
                    {
                        throw new InvalidDataException("Two users with the same id found in the database!");
                    }
                }
            }
            return idToReturn;
        }

        public bool PersistNewCamera(DatabaseCamera camera)
        {
            using (MySqlConnection conn = GetConnection())
            {
                string command = $"INSERT INTO {DatabaseCamera.TABLE_NAME} (" +
                               $"{DatabaseCamera.BRAND_LABEL},{DatabaseCamera.CAMERA_KEY_LABEL},{DatabaseCamera.CAMERA_NAME_LABEL}," +
                               $"{DatabaseCamera.IMAGE_PATH_LABEL},{DatabaseCamera.LOCATION_ID_LABEL},{DatabaseCamera.MODEL_LABEL}," +
                               $"{DatabaseCamera.USER_ID_LABEL},{DatabaseCamera.RESOLUTION_LABEL}" +
                               ") VALUES (" +
                               $"{formatNullableString(camera.Brand)},'{camera.CameraKey}',{formatNullableString(camera.CameraName)}," +
                               $"{formatNullableString(camera.ImagePath)},{formatNullableInt(camera.LocationId)},{formatNullableString(camera.Model)}," +
                               $"{formatNullableInt(camera.UserId)},{formatNullableString(camera.Resolution)}" +
                               ")";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(command, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool DeleteCameraFromCameraKey(string cameraKey)
        {
            if (String.IsNullOrWhiteSpace(cameraKey))
            {
                return false;
            }

            using (MySqlConnection conn = GetConnection())
            {
                string command = $"DELETE FROM {DatabaseCamera.TABLE_NAME} WHERE {DatabaseCamera.CAMERA_KEY_LABEL}='{cameraKey}'";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(command, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }

            // TODO: DELETE any associations that the camera had.

            return false;
        }

        public List<string> GetExistingCameraResolutions()
        {
            List<string> resolutionsInDB = new List<string>();
            using (MySqlConnection conn = GetConnection())
            {
                string query = "SELECT resolution FROM camera WHERE resolution IS NOT NULL GROUP BY resolution ORDER BY COUNT(*) DESC;";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resolutionsInDB.Add(Convert.ToString(reader["resolution"]));
                    }
                }
            }
            return resolutionsInDB;
        }

        private string formatNullableString(string nullableString)
        {
            return nullableString != null ? $"'{nullableString}'" : "NULL";
        }
        
        private string formatNullableInt(int? nullableInt)
        {
            return nullableInt != null ? $"{nullableInt.Value}" : "NULL";
        }
        
        private string formatNullableDate(DateTime? nullableDateTime)
        {
            return nullableDateTime != null ? $"'{nullableDateTime.Value.ToMySqlDateTimeString()}'" : "NULL";
        }

        #region API Key Service (API)

        // Performs a SELECT to query the specified API Key in the database.
        public DatabaseAPIKey GetAPIKeyFromId(int api_key_id)
        {
            DatabaseAPIKey api_key = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseAPIKey.TABLE_NAME} "
                    + $"WHERE {DatabaseAPIKey.API_KEY_ID_LABEL} = {api_key_id} LIMIT 1";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting only one row
                    if (reader.Read())
                    {
                        api_key = new DatabaseAPIKey()
                        {
                            APIKeyId = Convert.ToInt32(reader[DatabaseAPIKey.API_KEY_ID_LABEL]),
                            Key = Convert.ToString(reader[DatabaseAPIKey.API_KEY_LABEL]),
                            Salt = Convert.ToString(reader[DatabaseAPIKey.API_KEY_SALT_LABEL]),
                            IsActive = Convert.ToInt16(reader[DatabaseAPIKey.API_KEY_ISACTIVE_LABEL])
                        };
                    }
                }
            }
            return api_key;
        }

        // Performs a SELECT to query all API Keys in the database.
        public List<DatabaseAPIKey> GetAllAPIKeys()
        {
            List<DatabaseAPIKey> api_keys_list = new List<DatabaseAPIKey>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseAPIKey.TABLE_NAME}";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        api_keys_list.Add(new DatabaseAPIKey()
                        {
                            APIKeyId = Convert.ToInt32(reader[DatabaseAPIKey.API_KEY_ID_LABEL]),
                            Key = Convert.ToString(reader[DatabaseAPIKey.API_KEY_LABEL]),
                            Salt = Convert.ToString(reader[DatabaseAPIKey.API_KEY_SALT_LABEL]),
                            IsActive = Convert.ToInt32(reader[DatabaseAPIKey.API_KEY_ISACTIVE_LABEL]),
                            UserId = Convert.ToInt32(reader[DatabaseAPIKey.USER_ID_LABEL])
                        });
                    }
                }
            }
            return api_keys_list;
        }

        // Performs an INSERT to persist a newly created API Key (salted and hashed key and its Salt value).
        public bool PersistAPIKey(APIKey api_key)
        {
            int isKeyActive = (int)DatabaseAPIKey.API_Key_Status.INACTIVE;

            if (api_key.IsActive)
            {
                isKeyActive = (int)DatabaseAPIKey.API_Key_Status.ACTIVE;
            }

            // Define the insert command with the values to insert.
            string insertCommand = $"INSERT INTO {DatabaseAPIKey.TABLE_NAME} "
                + $"({DatabaseAPIKey.API_KEY_LABEL},{DatabaseAPIKey.API_KEY_SALT_LABEL},"
                + $"{DatabaseAPIKey.USER_ID_LABEL},{DatabaseAPIKey.API_KEY_ISACTIVE_LABEL}) VALUES "
                + $"('{api_key.API_Key}','{api_key.API_KeySalt}',"
                + $"{formatNullableInt(api_key.UserId)},{isKeyActive})";

            // Open connection and execute the insert command.
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(insertCommand, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return true;
        }

        // Performs an UPDATE to set the specified API key in the database to inactive. 
        public bool DeactivateAPIKey(int api_key_id)
        {
            DatabaseAPIKey api_key = GetAPIKeyFromId(api_key_id);
            
            if (api_key == null)
            {
                // Key was not found in database.
                return false;
            }
            else if (api_key.IsActive == (int)DatabaseAPIKey.API_Key_Status.INACTIVE)
            {
                // Key is already Inactive (Deactivated).
                return false;
            }

            // Define the update command with the values to update.
            string updateCommand = $"UPDATE {DatabaseAPIKey.TABLE_NAME} "
                + $"SET {DatabaseAPIKey.API_KEY_ISACTIVE_LABEL} = {(int)DatabaseAPIKey.API_Key_Status.INACTIVE} "
                + $"WHERE {DatabaseAPIKey.API_KEY_ID_LABEL} = {api_key_id}";

            // Open connection and execute the update command.
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                try
                {
                    MySqlCommand cmd = new MySqlCommand(updateCommand, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Write to Log
                    return false; // This is probably not going to be executed...
                }
            }
            return true;
        }
        #endregion

        public List<DatabaseCamera> GetCamerasOwnedByUser(int userId)
        {
            List<DatabaseCamera> cameraList = new List<DatabaseCamera>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseCamera.TABLE_NAME} " +
                               $"WHERE camera.user_id = {userId}";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseCamera camera = getDatabaseCameraFromReader(reader);
                        cameraList.Add(camera);
                    }
                }
            }
            return cameraList;
        }

        public List<DatabaseCamera> GetAllCameras()
        {
            List<DatabaseCamera> cameraList = new List<DatabaseCamera>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseCamera.TABLE_NAME}";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseCamera camera = getDatabaseCameraFromReader(reader);
                        cameraList.Add(camera);
                    }
                }
            }
            return cameraList;
        }

        public List<DatabaseCamera> GetCamerasAvailableToUser(int userId)
        {
            List<DatabaseCamera> cameraList = new List<DatabaseCamera>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = "SELECT * " +
                               "FROM camera " +
                               "WHERE camera.id IN ( " +
                               "SELECT user_camera_association.camera_id " +
                               "FROM user_camera_association " +
                               $"WHERE user_camera_association.user_id = {userId} " +
                               $") OR camera.user_id = {userId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseCamera camera = getDatabaseCameraFromReader(reader);
                        cameraList.Add(camera);
                    }
                }
            }
            return cameraList;
        }

        public bool PersistNewAlert(DatabaseAlert alert)
        {
            using (MySqlConnection conn = GetConnection())
            {
                if (alert.AlwaysActive)
                {
                    alert.StartTime = null;
                    alert.EndTime = null;
                }

                string query = $"INSERT INTO {DatabaseAlert.TABLE_NAME}(" +
                               $"{DatabaseAlert.ALERT_NAME_LABEL}, {DatabaseAlert.CAMERA_ID_LABEL}, {DatabaseAlert.USER_ID_LABEL}, " +
                               $"{DatabaseAlert.CONTACT_METHOD_LABEL}, {DatabaseAlert.TRIGGER_OPERATOR_LABEL}, {DatabaseAlert.TRIGGER_NUMBER_LABEL}, " +
                               $"{DatabaseAlert.ALWAYS_ACTIVE_LABEL}, {DatabaseAlert.START_TIME_LABEL}, {DatabaseAlert.END_TIME_LABEL}, " +
                               $"{DatabaseAlert.SNOOZED_UNTIL_LABEL}, {DatabaseAlert.DISABLED_UNTIL_LABEL}" +
                               ") VALUES " +
                               $"('{alert.AlertName}',{alert.CameraId}," +
                               $"{alert.UserId},'{alert.ContactMethod}','{alert.TriggerOperator}'," +
                               $"{alert.TriggerNumber},{(alert.AlwaysActive ? 1 : 0)}," +
                               $"{formatNullableString(alert.StartTime)}, {formatNullableString(alert.EndTime)}, " +
                               $"{formatNullableDate(alert.SnoozedUntil)}, {formatNullableDate(alert.DisabledUntil)});";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public List<DatabaseAlert> GetAllAlerts(int userId = 0)
        {
            List<DatabaseAlert> alertList = new List<DatabaseAlert>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = "SELECT * " +
                               $"FROM {DatabaseAlert.TABLE_NAME}";

                if (userId != 0)
                {
                    query += $" WHERE {DatabaseAlert.USER_ID_LABEL} = {userId};";
                }
                else
                {
                    query += ";";
                }

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseAlert alert = new DatabaseAlert
                        {
                            AlertId = Convert.ToInt32(reader[DatabaseAlert.ALERT_ID_LABEL]),
                            CameraId = Convert.ToInt32(reader[DatabaseAlert.CAMERA_ID_LABEL]),
                            UserId = Convert.ToInt32(reader[DatabaseAlert.USER_ID_LABEL]),
                            AlertName = Convert.ToString(reader[DatabaseAlert.ALERT_NAME_LABEL]),
                            ContactMethod = Convert.ToString(reader[DatabaseAlert.CONTACT_METHOD_LABEL]),
                            TriggerOperator = Convert.ToString(reader[DatabaseAlert.TRIGGER_OPERATOR_LABEL]),
                            TriggerNumber = Convert.ToInt32(reader[DatabaseAlert.TRIGGER_NUMBER_LABEL]),
                            AlwaysActive = Convert.ToBoolean(reader[DatabaseAlert.ALWAYS_ACTIVE_LABEL])
                        };
                        if (reader[DatabaseAlert.START_TIME_LABEL] != DBNull.Value)
                        {
                            alert.StartTime = Convert.ToString(reader[DatabaseAlert.START_TIME_LABEL]);
                        }
                        if (reader[DatabaseAlert.END_TIME_LABEL] != DBNull.Value)
                        {
                            alert.EndTime = Convert.ToString(reader[DatabaseAlert.END_TIME_LABEL]);
                        }
                        if (reader[DatabaseAlert.DISABLED_UNTIL_LABEL] != DBNull.Value)
                        {
                            alert.DisabledUntil = Convert.ToDateTime(reader[DatabaseAlert.DISABLED_UNTIL_LABEL]);
                        }
                        if (reader[DatabaseAlert.SNOOZED_UNTIL_LABEL] != DBNull.Value)
                        {
                            alert.SnoozedUntil = Convert.ToDateTime(reader[DatabaseAlert.SNOOZED_UNTIL_LABEL]);
                        }
                        alertList.Add(alert);
                    }
                }
            }
            return alertList;
        }
        
        public bool DeleteAlert(int alertId)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                string query = $"DELETE FROM {DatabaseAlert.TABLE_NAME} " +
                               $"WHERE {DatabaseAlert.ALERT_ID_LABEL} = {alertId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PersistExistingAlert(DatabaseAlert alert)
        {
            using (MySqlConnection conn = GetConnection())
            {
                if (alert.AlwaysActive)
                {
                    alert.StartTime = null;
                    alert.EndTime = null;
                }

                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $"UPDATE {DatabaseAlert.TABLE_NAME} SET " +
                               $"{DatabaseAlert.ALERT_NAME_LABEL} = {formatNullableString(alert.AlertName)}," +
                               $"{DatabaseAlert.CAMERA_ID_LABEL} = {alert.CameraId}," +
                               $"{DatabaseAlert.CONTACT_METHOD_LABEL} = {formatNullableString(alert.ContactMethod)}," +
                               $"{DatabaseAlert.TRIGGER_OPERATOR_LABEL} = {formatNullableString(alert.TriggerOperator)}," +
                               $"{DatabaseAlert.TRIGGER_NUMBER_LABEL} = {alert.TriggerNumber}," +
                               $"{DatabaseAlert.ALWAYS_ACTIVE_LABEL} = {alert.AlwaysActive}," +
                               $"{DatabaseAlert.START_TIME_LABEL} = {formatNullableString(alert.StartTime)}," +
                               $"{DatabaseAlert.END_TIME_LABEL} = {formatNullableString(alert.EndTime)}," +
                               $"{DatabaseAlert.DISABLED_UNTIL_LABEL} = {formatNullableDate(alert.DisabledUntil)}," +
                               $"{DatabaseAlert.SNOOZED_UNTIL_LABEL} = {formatNullableDate(alert.SnoozedUntil)} " +
                               $"WHERE {DatabaseAlert.ALERT_ID_LABEL} = {alert.AlertId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PersistNewLocation(DatabaseLocation dbLocation)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                string query = $"INSERT INTO {DatabaseLocation.TABLE_NAME}(" +
                               $"{DatabaseLocation.LOCATION_NAME_LABEL}, {DatabaseLocation.ADDRESS_LINE_LABEL}, " +
                               $"{DatabaseLocation.CITY_LABEL}, {DatabaseLocation.STATE_LABEL}, {DatabaseLocation.ZIP_LABEL}" +
                               ") VALUES " +
                               $"('{dbLocation.LocationName}',{formatNullableString(dbLocation.AddressLine)}," +
                               $"{formatNullableString(dbLocation.City)},{formatNullableString(dbLocation.State)}," +
                               $"{formatNullableString(dbLocation.Zip)});";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public DatabasePerSecondStat GetEarliestPerSecondStatTriggeringAlert(DatabaseAlert alert,
            DateTime lastUpdatedTime, DateTime checkupDateTime)
        {
            DatabasePerSecondStat perSecondStat = null;
            TriggerOperator triggerOperator = (TriggerOperator)Enum.Parse(typeof(TriggerOperator), alert.TriggerOperator, true);
            
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabasePerSecondStat.TABLE_NAME} " + 
                               $"WHERE {DatabasePerSecondStat.NUM_DETECTED_OBJECTS_LABEL} " +
                               $"{triggerOperator.GetSqlForm()} {alert.TriggerNumber} " +
                               $"AND {DatabasePerSecondStat.CAMERA_ID_LABEL} = {alert.CameraId} " +
                               $"AND {DatabasePerSecondStat.DATE_TIME_LABEL} > STR_TO_DATE('{lastUpdatedTime}', '%m/%d/%Y %H:%i:%s')" +
                               $"AND {DatabasePerSecondStat.DATE_TIME_LABEL} < STR_TO_DATE('{checkupDateTime}', '%m/%d/%Y %H:%i:%s')";
                
                if (alert.SnoozedUntil != null)
                {
                    query += $" AND {DatabasePerSecondStat.DATE_TIME_LABEL} >= STR_TO_DATE('{alert.SnoozedUntil.Value}', '%m/%d/%Y %H:%i:%s')";
                }
                if (alert.DisabledUntil != null)
                {
                    query += $" AND {DatabasePerSecondStat.DATE_TIME_LABEL} >= STR_TO_DATE('{alert.DisabledUntil.Value}', '%m/%d/%Y %H:%i:%s')";
                }
                if (!alert.AlwaysActive)
                {
                    query += $" AND CAST({DatabasePerSecondStat.DATE_TIME_LABEL} as TIME) >= '{alert.StartTime}'" +
                             $" AND CAST({DatabasePerSecondStat.DATE_TIME_LABEL} as TIME) <= '{alert.EndTime}'";
                }
                query += $" ORDER BY {DatabasePerSecondStat.DATE_TIME_LABEL} ASC LIMIT 1;";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        perSecondStat = new DatabasePerSecondStat
                        {
                            PerSecondStatId = Convert.ToInt32(reader[DatabasePerSecondStat.PER_SECOND_STAT_ID_LABEL]),
                            DateTime = Convert.ToDateTime(reader[DatabasePerSecondStat.DATE_TIME_LABEL]),
                            CameraId = Convert.ToInt32(reader[DatabasePerSecondStat.CAMERA_ID_LABEL]),
                            NumDetectedObjects = Convert.ToInt32(reader[DatabasePerSecondStat.NUM_DETECTED_OBJECTS_LABEL]),
                            HasSavedImage = Convert.ToBoolean(Convert.ToInt16(reader[DatabasePerSecondStat.HAS_SAVED_IMAGE_LABEL])),
                            // Null by default, this is what will be updated later by the HourlyStatsService.
                            PerHourStatId = -1
                        };
                    }
                }
            }
            return perSecondStat;
        }

        public List<DatabaseNotification> GetNotificationsForUser(int userId)
        {
            List<DatabaseNotification> notificationList = new List<DatabaseNotification>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = "SELECT * " +
                               $"FROM {DatabaseNotification.TABLE_NAME} " +
                               $"WHERE {DatabaseNotification.ALERT_ID_LABEL} IN ( " +
                               "SELECT alert.id " +
                               "FROM alert " +
                               $"WHERE alert.user_id = {userId});";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseNotification notification = new DatabaseNotification
                        {
                            NotificationId = Convert.ToInt32(reader[DatabaseNotification.NOTIFICATION_ID_LABEL]),
                            AlertId = Convert.ToInt32(reader[DatabaseNotification.ALERT_ID_LABEL]),
                            Acknowledged = Convert.ToBoolean(reader[DatabaseNotification.ACKNOWLEDGED_LABEL]),
                            TriggerDateTime = Convert.ToDateTime(reader[DatabaseNotification.TRIGGER_DATETIME_LABEL]),
                            FailedEmail = Convert.ToBoolean(reader[DatabaseNotification.FAILED_EMAIL_LABEL])
                        };
                        notificationList.Add(notification);
                    }
                }
            }
            return notificationList;
        }

        public List<DatabaseAlert> GetAlertsById(List<int> alertIds)
        {
            List<DatabaseAlert> alertList = new List<DatabaseAlert>();

            if (alertIds.Count > 0)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    string query = "SELECT * " +
                                   $"FROM {DatabaseAlert.TABLE_NAME} " +
                                   $"WHERE {DatabaseAlert.ALERT_ID_LABEL} IN " +
                                   $"({string.Join(",", alertIds)});";
                    
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DatabaseAlert alert = new DatabaseAlert
                            {
                                AlertId = Convert.ToInt32(reader[DatabaseAlert.ALERT_ID_LABEL]),
                                CameraId = Convert.ToInt32(reader[DatabaseAlert.CAMERA_ID_LABEL]),
                                UserId = Convert.ToInt32(reader[DatabaseAlert.USER_ID_LABEL]),
                                AlertName = Convert.ToString(reader[DatabaseAlert.ALERT_NAME_LABEL]),
                                ContactMethod = Convert.ToString(reader[DatabaseAlert.CONTACT_METHOD_LABEL]),
                                TriggerOperator = Convert.ToString(reader[DatabaseAlert.TRIGGER_OPERATOR_LABEL]),
                                TriggerNumber = Convert.ToInt32(reader[DatabaseAlert.TRIGGER_NUMBER_LABEL]),
                                AlwaysActive = Convert.ToBoolean(reader[DatabaseAlert.ALWAYS_ACTIVE_LABEL])
                            };
                            if (reader[DatabaseAlert.START_TIME_LABEL] != DBNull.Value)
                            {
                                alert.StartTime = Convert.ToString(reader[DatabaseAlert.START_TIME_LABEL]);
                            }
                            if (reader[DatabaseAlert.END_TIME_LABEL] != DBNull.Value)
                            {
                                alert.EndTime = Convert.ToString(reader[DatabaseAlert.END_TIME_LABEL]);
                            }
                            if (reader[DatabaseAlert.DISABLED_UNTIL_LABEL] != DBNull.Value)
                            {
                                alert.DisabledUntil = Convert.ToDateTime(reader[DatabaseAlert.DISABLED_UNTIL_LABEL]);
                            }
                            if (reader[DatabaseAlert.SNOOZED_UNTIL_LABEL] != DBNull.Value)
                            {
                                alert.SnoozedUntil = Convert.ToDateTime(reader[DatabaseAlert.SNOOZED_UNTIL_LABEL]);
                            }
                            alertList.Add(alert);
                        }
                    }
                }
            }
            return alertList;
        }

        public DatabaseNotification GetNotificationById(int notificationId)
        {
            DatabaseNotification notification = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseNotification.TABLE_NAME} WHERE {DatabaseNotification.NOTIFICATION_ID_LABEL} = {notificationId}";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        notification = new DatabaseNotification
                        {
                            NotificationId = Convert.ToInt32(reader[DatabaseNotification.NOTIFICATION_ID_LABEL]),
                            AlertId = Convert.ToInt32(reader[DatabaseNotification.ALERT_ID_LABEL]),
                            Acknowledged = Convert.ToBoolean(reader[DatabaseNotification.ACKNOWLEDGED_LABEL]),
                            TriggerDateTime = Convert.ToDateTime(reader[DatabaseNotification.TRIGGER_DATETIME_LABEL]),
                            FailedEmail = Convert.ToBoolean(reader[DatabaseNotification.FAILED_EMAIL_LABEL])
                        };
                    }
                }
            }

            return notification;
        }

        public bool AcknowledgeNotification(int notificationId)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                string query = $"UPDATE {DatabaseNotification.TABLE_NAME} " +
                               $"SET {DatabaseNotification.ACKNOWLEDGED_LABEL}=1 " +
                               $"WHERE {DatabaseNotification.NOTIFICATION_ID_LABEL} = {notificationId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public DatabaseAlert GetAlertById(int alertId)
        {
            DatabaseAlert alert = null;
            
            using (MySqlConnection conn = GetConnection())
            {
                string query = "SELECT * " +
                               $"FROM {DatabaseAlert.TABLE_NAME} " +
                               $"WHERE {DatabaseAlert.ALERT_ID_LABEL} = {alertId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        alert = new DatabaseAlert
                        {
                            AlertId = Convert.ToInt32(reader[DatabaseAlert.ALERT_ID_LABEL]),
                            CameraId = Convert.ToInt32(reader[DatabaseAlert.CAMERA_ID_LABEL]),
                            UserId = Convert.ToInt32(reader[DatabaseAlert.USER_ID_LABEL]),
                            AlertName = Convert.ToString(reader[DatabaseAlert.ALERT_NAME_LABEL]),
                            ContactMethod = Convert.ToString(reader[DatabaseAlert.CONTACT_METHOD_LABEL]),
                            TriggerOperator = Convert.ToString(reader[DatabaseAlert.TRIGGER_OPERATOR_LABEL]),
                            TriggerNumber = Convert.ToInt32(reader[DatabaseAlert.TRIGGER_NUMBER_LABEL]),
                            AlwaysActive = Convert.ToBoolean(reader[DatabaseAlert.ALWAYS_ACTIVE_LABEL])
                        };
                        if (reader[DatabaseAlert.START_TIME_LABEL] != DBNull.Value)
                        {
                            alert.StartTime = Convert.ToString(reader[DatabaseAlert.START_TIME_LABEL]);
                        }
                        if (reader[DatabaseAlert.END_TIME_LABEL] != DBNull.Value)
                        {
                            alert.EndTime = Convert.ToString(reader[DatabaseAlert.END_TIME_LABEL]);
                        }
                        if (reader[DatabaseAlert.DISABLED_UNTIL_LABEL] != DBNull.Value)
                        {
                            alert.DisabledUntil = Convert.ToDateTime(reader[DatabaseAlert.DISABLED_UNTIL_LABEL]);
                        }
                        if (reader[DatabaseAlert.SNOOZED_UNTIL_LABEL] != DBNull.Value)
                        {
                            alert.SnoozedUntil = Convert.ToDateTime(reader[DatabaseAlert.SNOOZED_UNTIL_LABEL]);
                        }
                    }
                }
            }
            return alert;
        }

        public DatabaseLocation GetLocationById(int locationId)
        {
            DatabaseLocation location = new DatabaseLocation();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseLocation.TABLE_NAME} " +
                               $"WHERE {DatabaseLocation.LOCATION_ID_LABEL} = {locationId}";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        location = new DatabaseLocation
                        {
                            LocationId = Convert.ToInt32(reader[DatabaseLocation.LOCATION_ID_LABEL]),
                            LocationName = Convert.ToString(reader[DatabaseLocation.LOCATION_NAME_LABEL]),
                            AddressLine = Convert.ToString(reader[DatabaseLocation.ADDRESS_LINE_LABEL]),
                            City = Convert.ToString(reader[DatabaseLocation.CITY_LABEL]),
                            State = Convert.ToString(reader[DatabaseLocation.STATE_LABEL]),
                            Zip = Convert.ToString(reader[DatabaseLocation.ZIP_LABEL])
                        };
                    }
                }
            }
            return location;
        }

        public bool PersistNewNotification(DatabaseNotification dbNotification)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                string query = $"INSERT INTO {DatabaseNotification.TABLE_NAME}(" +
                               $"{DatabaseNotification.ALERT_ID_LABEL}, {DatabaseNotification.TRIGGER_DATETIME_LABEL}, " +
                               $"{DatabaseNotification.ACKNOWLEDGED_LABEL}, {DatabaseNotification.FAILED_EMAIL_LABEL}" +
                               ") VALUES " +
                               $"({dbNotification.AlertId},'{dbNotification.TriggerDateTime.ToMySqlDateTimeString()}',{dbNotification.Acknowledged},{dbNotification.FailedEmail});";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PersistExistingCameraByCameraKey(DatabaseCamera databaseCamera, bool imageDeleted)
        {
            using (MySqlConnection conn = GetConnection())
            {
                if (databaseCamera.ImagePath != null)
                {
                    databaseCamera.ImagePath = databaseCamera.ImagePath.Replace("\\", "/");
                }

                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $"UPDATE {DatabaseCamera.TABLE_NAME} SET " +
                               $"{DatabaseCamera.CAMERA_NAME_LABEL} = {formatNullableString(databaseCamera.CameraName)}," +
                               $"{DatabaseCamera.RESOLUTION_LABEL} = {formatNullableString(databaseCamera.Resolution)}," +
                               $"{DatabaseCamera.BRAND_LABEL} = {formatNullableString(databaseCamera.Brand)}," +
                               $"{DatabaseCamera.MODEL_LABEL} = {formatNullableString(databaseCamera.Model)}," +
                               $"{DatabaseCamera.LOCATION_ID_LABEL} = {formatNullableInt(databaseCamera.LocationId)}," +
                               $"{DatabaseCamera.USER_ID_LABEL} = {formatNullableInt(databaseCamera.UserId)}," +
                               $"{DatabaseCamera.ROOM_ID_LABEL} = {formatNullableInt(databaseCamera.RoomId)}";

                if (imageDeleted)
                {
                    query += $", {DatabaseCamera.IMAGE_PATH_LABEL} = NULL";
                }
                else if (!databaseCamera.ImagePath.IsNullOrEmpty())
                {
                    query += $", {DatabaseCamera.IMAGE_PATH_LABEL} = '{databaseCamera.ImagePath}'";
                }

                query += $" WHERE {DatabaseCamera.CAMERA_KEY_LABEL} = '{databaseCamera.CameraKey}';";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public List<DatabaseCamera> GetCamerasForLocationForUser(int locationId, int userId)
        {
            List<DatabaseCamera> cameraList = new List<DatabaseCamera>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseCamera.TABLE_NAME} " +
                               $"WHERE {DatabaseCamera.LOCATION_ID_LABEL} = {locationId} " +
                               "AND (" +
                                   $"{DatabaseCamera.CAMERA_ID_LABEL} IN ( " +
                                       "SELECT user_camera_association.camera_id " +
                                       "FROM user_camera_association " +
                                       $"WHERE user_camera_association.user_id = {userId} " +
                                   $") OR {DatabaseCamera.USER_ID_LABEL} = {userId}" +
                               ");";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseCamera camera = getDatabaseCameraFromReader(reader);
                        cameraList.Add(camera);
                    }
                }
            }
            return cameraList;
        }

        private DatabaseCamera getDatabaseCameraFromReader(MySqlDataReader reader)
        {
            DatabaseCamera camera = new DatabaseCamera
            {
                CameraKey = Convert.ToString(reader[DatabaseCamera.CAMERA_KEY_LABEL]),
                CameraId = Convert.ToInt32(reader[DatabaseCamera.CAMERA_ID_LABEL])
            };
            if (reader[DatabaseCamera.CAMERA_NAME_LABEL] != DBNull.Value)
            {
                camera.CameraName = Convert.ToString(reader[DatabaseCamera.CAMERA_NAME_LABEL]);
            }
            if (reader[DatabaseCamera.MODEL_LABEL] != DBNull.Value)
            {
                camera.Model = Convert.ToString(reader[DatabaseCamera.MODEL_LABEL]);
            }
            if (reader[DatabaseCamera.BRAND_LABEL] != DBNull.Value)
            {
                camera.Brand = Convert.ToString(reader[DatabaseCamera.BRAND_LABEL]);
            }
            if (reader[DatabaseCamera.RESOLUTION_LABEL] != DBNull.Value)
            {
                camera.Resolution = Convert.ToString(reader[DatabaseCamera.RESOLUTION_LABEL]);
            }
            if (reader[DatabaseCamera.USER_ID_LABEL] != DBNull.Value)
            {
                camera.UserId = Convert.ToInt32(reader[DatabaseCamera.USER_ID_LABEL]);
            }
            if (reader[DatabaseCamera.LOCATION_ID_LABEL] != DBNull.Value)
            {
                camera.LocationId = Convert.ToInt32(reader[DatabaseCamera.LOCATION_ID_LABEL]);
            }
            if (reader[DatabaseCamera.ROOM_ID_LABEL] != DBNull.Value)
            {
                camera.RoomId = Convert.ToInt32(reader[DatabaseCamera.ROOM_ID_LABEL]);
            }
            if (reader[DatabaseCamera.IMAGE_PATH_LABEL] != DBNull.Value)
            {
                camera.ImagePath = Convert.ToString(reader[DatabaseCamera.IMAGE_PATH_LABEL]);
            }

            return camera;
        }
        
        public bool IsUserAdministrator(int userId)
        {
            DatabaseUser user = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT {DatabaseUser.IS_ADMINISTRATOR_LABEL} " +
                               $"FROM {DatabaseUser.TABLE_NAME} WHERE {DatabaseUser.USER_ID_LABEL} = {userId} " +
                               "LIMIT 1;";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        return Convert.ToBoolean(reader[DatabaseUser.IS_ADMINISTRATOR_LABEL]);
                    }
                }
            }

            return false;
        }
        
        public DatabaseUser GetUserById(int userId)
        {
            DatabaseUser user = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseUser.TABLE_NAME} WHERE {DatabaseUser.USER_ID_LABEL} = {userId}";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        user = new DatabaseUser
                        {
                            UserId = Convert.ToInt32(reader[DatabaseUser.USER_ID_LABEL]),
                            Username = Convert.ToString(reader[DatabaseUser.USERNAME_LABEL]),
                            Password = Convert.ToString(reader[DatabaseUser.PASSWORD_LABEL]),
                            EmailAddress = Convert.ToString(reader[DatabaseUser.EMAIL_ADDRESS_LABEL]),
                            FirstName = Convert.ToString(reader[DatabaseUser.FIRST_NAME_LABEL]),
                            LastName = Convert.ToString(reader[DatabaseUser.LAST_NAME_LABEL]),
                            IsAdministrator = Convert.ToBoolean(reader[DatabaseUser.IS_ADMINISTRATOR_LABEL])
                        };
                    }
                }
            }

            return user;
        }
        
        public List<DatabaseUser> GetAllUsers()
        {
            List<DatabaseUser> userList = new List<DatabaseUser>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseUser.TABLE_NAME}";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseUser user = new DatabaseUser
                        {
                            UserId = Convert.ToInt32(reader[DatabaseUser.USER_ID_LABEL]),
                            Username = Convert.ToString(reader[DatabaseUser.USERNAME_LABEL]),
                            Password = Convert.ToString(reader[DatabaseUser.PASSWORD_LABEL]),
                            EmailAddress = Convert.ToString(reader[DatabaseUser.EMAIL_ADDRESS_LABEL]),
                            FirstName = Convert.ToString(reader[DatabaseUser.FIRST_NAME_LABEL]),
                            LastName = Convert.ToString(reader[DatabaseUser.LAST_NAME_LABEL]),
                            IsAdministrator = Convert.ToBoolean(reader[DatabaseUser.IS_ADMINISTRATOR_LABEL])
                        };
                        userList.Add(user);
                    }
                }
            }
            return userList;
        }
        
        public DatabaseUser GetUserByUsername(string username)
        {
            DatabaseUser user = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseUser.TABLE_NAME} WHERE {DatabaseUser.USERNAME_LABEL} = '{username}';";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        user = new DatabaseUser
                        {
                            UserId = Convert.ToInt32(reader[DatabaseUser.USER_ID_LABEL]),
                            Username = Convert.ToString(reader[DatabaseUser.USERNAME_LABEL]),
                            Password = Convert.ToString(reader[DatabaseUser.PASSWORD_LABEL]),
                            EmailAddress = Convert.ToString(reader[DatabaseUser.EMAIL_ADDRESS_LABEL]),
                            FirstName = Convert.ToString(reader[DatabaseUser.FIRST_NAME_LABEL]),
                            LastName = Convert.ToString(reader[DatabaseUser.LAST_NAME_LABEL]),
                            IsAdministrator = Convert.ToBoolean(reader[DatabaseUser.IS_ADMINISTRATOR_LABEL])
                        };
                    }
                }
            }

            return user;
        }

        public DatabaseUser GetUserByEmailAddress(string email)
        {
            DatabaseUser user = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseUser.TABLE_NAME} WHERE {DatabaseUser.EMAIL_ADDRESS_LABEL} = '{email}';";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        user = new DatabaseUser
                        {
                            UserId = Convert.ToInt32(reader[DatabaseUser.USER_ID_LABEL]),
                            Username = Convert.ToString(reader[DatabaseUser.USERNAME_LABEL]),
                            Password = Convert.ToString(reader[DatabaseUser.PASSWORD_LABEL]),
                            EmailAddress = Convert.ToString(reader[DatabaseUser.EMAIL_ADDRESS_LABEL]),
                            FirstName = Convert.ToString(reader[DatabaseUser.FIRST_NAME_LABEL]),
                            LastName = Convert.ToString(reader[DatabaseUser.LAST_NAME_LABEL]),
                            IsAdministrator = Convert.ToBoolean(reader[DatabaseUser.IS_ADMINISTRATOR_LABEL])
                        };
                    }
                }
            }

            return user;
        }

        public DatabaseUser GetUserByPasswordResetToken(string token)
        {
            DatabaseUser user = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseUser.TABLE_NAME} WHERE {DatabaseUser.PASSWORD_RESET_TOKEN_LABEL} = '{token}';";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        user = new DatabaseUser
                        {
                            UserId = Convert.ToInt32(reader[DatabaseUser.USER_ID_LABEL]),
                            Username = Convert.ToString(reader[DatabaseUser.USERNAME_LABEL]),
                            Password = Convert.ToString(reader[DatabaseUser.PASSWORD_LABEL]),
                            EmailAddress = Convert.ToString(reader[DatabaseUser.EMAIL_ADDRESS_LABEL]),
                            FirstName = Convert.ToString(reader[DatabaseUser.FIRST_NAME_LABEL]),
                            LastName = Convert.ToString(reader[DatabaseUser.LAST_NAME_LABEL]),
                            IsAdministrator = Convert.ToBoolean(reader[DatabaseUser.IS_ADMINISTRATOR_LABEL])
                        };
                    }
                }
            }

            return user;
        }

        public bool PersistPasswordResetToken(string passwordResetToken, string emailAddress)
        {
            using (MySqlConnection conn = GetConnection())
            {
                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $"UPDATE {DatabaseUser.TABLE_NAME} SET " +
                               $"{DatabaseUser.PASSWORD_RESET_TOKEN_LABEL} = {formatNullableString(passwordResetToken)} " +
                               $"WHERE {DatabaseUser.EMAIL_ADDRESS_LABEL} = '{emailAddress}';";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool PersistRemovePasswordResetToken(int userId)
        {
            using (MySqlConnection conn = GetConnection())
            {
                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $"UPDATE {DatabaseUser.TABLE_NAME} SET " +
                               $"{DatabaseUser.PASSWORD_RESET_TOKEN_LABEL} = NULL " +
                               $"WHERE {DatabaseUser.USER_ID_LABEL} = {userId};";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PersistExistingUser(DatabaseUser databaseUser)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $"UPDATE {DatabaseUser.TABLE_NAME} SET " +
                               $"{DatabaseUser.FIRST_NAME_LABEL} = {formatNullableString(databaseUser.FirstName)}," +
                               $"{DatabaseUser.LAST_NAME_LABEL} = {formatNullableString(databaseUser.LastName)}," +
                               $"{DatabaseUser.EMAIL_ADDRESS_LABEL} = {formatNullableString(databaseUser.EmailAddress)} " +
                               $"WHERE {DatabaseUser.USER_ID_LABEL} = {databaseUser.UserId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PersistPasswordChange(DatabaseUser databaseUser)
        {
            using (MySqlConnection conn = GetConnection())
            {
                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $"UPDATE {DatabaseUser.TABLE_NAME} SET " +
                               $"{DatabaseUser.PASSWORD_LABEL} = {formatNullableString(databaseUser.Password)} " +
                               $"WHERE {DatabaseUser.USER_ID_LABEL} = {databaseUser.UserId};";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool PersistNewUser(DatabaseUser databaseUser)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $" INSERT INTO {DatabaseUser.TABLE_NAME}(" +
                               $"{DatabaseUser.USERNAME_LABEL}," +
                               $"{DatabaseUser.FIRST_NAME_LABEL}," +
                               $"{DatabaseUser.LAST_NAME_LABEL}," +
                               $"{DatabaseUser.EMAIL_ADDRESS_LABEL}," +
                               $"{DatabaseUser.PASSWORD_LABEL}" +
                               $") VALUES "+
                               $"('{databaseUser.Username}', {formatNullableString(databaseUser.FirstName)}, " +
                               $"{formatNullableString(databaseUser.LastName)}, {formatNullableString(databaseUser.EmailAddress)}, " +
                               $"'{databaseUser.Password}');";

                conn.Open();     
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }
        
        public DatabaseGraphStat getGraphStatByTimeInterval(int cameraID, DateTime start, DateTime end)
        {
            DatabaseGraphStat stat = null;

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT AVG({DatabaseGraphStat.NUM_DETECTED_OBJECTS_LABEL}) as avg_num," +
                               $"MAX({ DatabaseGraphStat.NUM_DETECTED_OBJECTS_LABEL}) as max_num, " +
                               $"MIN({ DatabaseGraphStat.NUM_DETECTED_OBJECTS_LABEL}) as min_num, " +
                               $"{DatabaseGraphStat.CAMERA_ID_LABEL} " +
                               $"FROM {DatabaseGraphStat.TABLE_NAME} " +
                               //$"WHERE {DatabaseGraphStat.DATE_TIME_LABEL} between '2018-04-11 14:36:38' AND '2018-04-11 14:36:43' " +
                               $"WHERE {DatabaseGraphStat.DATE_TIME_LABEL} between '{start.ToString("yyyy-MM-dd HH:mm:ss")}' AND '{end.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                               $"AND {DatabaseGraphStat.CAMERA_ID_LABEL} = {cameraID}";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stat = new DatabaseGraphStat
                        {
                            CameraID = Convert.ToString(reader[DatabaseGraphStat.CAMERA_ID_LABEL]),
                            Start = start,
                            End = end
                        };
                        if (reader["avg_num"] != DBNull.Value)
                        {
                            stat.AverageDetectedObjects = Convert.ToDouble(reader["avg_num"]);
                        }
                        if (reader["max_num"] != DBNull.Value) {
                            stat.MaximumDetectedObjects = Convert.ToInt32(reader["max_num"]);
                        }
                        if (reader["min_num"] != DBNull.Value) {
                            stat.MinimumDetectedObjects = Convert.ToInt32(reader["min_num"]);
                        }
                        
                    }
                }
            }
            return stat;
        }

        public List<DatabaseLocation> GetLocations()
        {
            List<DatabaseLocation> locationList = new List<DatabaseLocation>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseLocation.TABLE_NAME};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        locationList.Add(new DatabaseLocation
                        {
                            LocationId = Convert.ToInt32(reader[DatabaseLocation.LOCATION_ID_LABEL]),
                            LocationName = Convert.ToString(reader[DatabaseLocation.LOCATION_NAME_LABEL]),
                            AddressLine = Convert.ToString(reader[DatabaseLocation.ADDRESS_LINE_LABEL]),
                            City = Convert.ToString(reader[DatabaseLocation.CITY_LABEL]),
                            State = Convert.ToString(reader[DatabaseLocation.STATE_LABEL]),
                            Zip = Convert.ToString(reader[DatabaseLocation.ZIP_LABEL])
                        });
                    }
                }
            }
            return locationList;
        }

        public List<DatabaseRoom> GetRoomsAtLocation(int locationId)
        {
            List<DatabaseRoom> roomList = new List<DatabaseRoom>();            
            
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseRoom.TABLE_NAME} "
                               + $"WHERE {DatabaseRoom.LOCATION_ID_LABEL} = {locationId}";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    while (reader.Read())
                    {
                        roomList.Add(new DatabaseRoom
                        {
                            RoomId = Convert.ToInt32(reader[DatabaseRoom.ROOM_ID_LABEL]),
                            LocationId = Convert.ToInt32(reader[DatabaseRoom.LOCATION_ID_LABEL]),
                            RoomName = Convert.ToString(reader[DatabaseRoom.ROOM_NAME_LABEL])
                        });
                    }
                }
            }

            return roomList;
        }

        public bool PersistNewRoom(DatabaseRoom databaseRoom)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $" INSERT INTO {DatabaseRoom.TABLE_NAME}(" +
                               $"{DatabaseRoom.LOCATION_ID_LABEL}," +
                               $"{DatabaseRoom.ROOM_NAME_LABEL}" +
                               ") VALUES "+
                               $"('{databaseRoom.LocationId}', '{databaseRoom.RoomName}');";

                conn.Open();     
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetRoomIdByLocationIdAndRoomName(int locationId, string roomName)
        {
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT {DatabaseRoom.ROOM_ID_LABEL} FROM {DatabaseRoom.TABLE_NAME} " + 
                               $"WHERE {DatabaseRoom.LOCATION_ID_LABEL} = {locationId} " +
                               $"AND {DatabaseRoom.ROOM_NAME_LABEL} = '{roomName}' LIMIT 1;";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader[DatabaseRoom.ROOM_ID_LABEL]);
                    }
                }
            }

            return -1;
        }

        public DatabaseRoom GetRoomById(int cameraRoomId)
        {
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseRoom.TABLE_NAME} " + 
                               $"WHERE {DatabaseRoom.ROOM_ID_LABEL} = {cameraRoomId} LIMIT 1;";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        return new DatabaseRoom
                        {
                            RoomId = Convert.ToInt32(reader[DatabaseRoom.ROOM_ID_LABEL]),
                            LocationId = Convert.ToInt32(reader[DatabaseRoom.ROOM_ID_LABEL]),
                            RoomName = Convert.ToString(reader[DatabaseRoom.ROOM_NAME_LABEL])
                        };
                    }
                }
            }

            return null;
        }

        public List<DatabaseCamera> GetAllCamerasInRoom(int roomId)
        {
            List<DatabaseCamera> cameraList = new List<DatabaseCamera>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseCamera.TABLE_NAME} " +
                               $"WHERE {DatabaseCamera.ROOM_ID_LABEL} = {roomId};";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseCamera camera = getDatabaseCameraFromReader(reader);
                        cameraList.Add(camera);
                    }
                }
            }
            return cameraList;
        }

        public bool PersistNewAPIKey(DatabaseAPIKey apiKey)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                string query = $"INSERT INTO {DatabaseAPIKey.TABLE_NAME}(" +
                               $"{DatabaseAPIKey.API_KEY_LABEL}, {DatabaseAPIKey.API_KEY_ISACTIVE_LABEL}, " +
                               $"{DatabaseAPIKey.API_KEY_SALT_LABEL}, {DatabaseAPIKey.USER_ID_LABEL}" +
                               ") VALUES " +
                               $"('{apiKey.Key}',{apiKey.IsActive}," +
                               $"'{apiKey.Salt}',{apiKey.UserId});";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public string GetCameraKeyFromId(int cameraId)
        {
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT {DatabaseCamera.CAMERA_KEY_LABEL} FROM {DatabaseCamera.TABLE_NAME} "
                               + $"WHERE {DatabaseCamera.CAMERA_ID_LABEL} = {cameraId} LIMIT 1";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting one result.
                    if (reader.Read())
                    {
                        return Convert.ToString(reader[DatabaseCamera.CAMERA_KEY_LABEL]);
                    }
                }
            }

            return null;
        }

        public bool CreateUserCameraAssociation(int userId, int cameraId)
        {
            using (MySqlConnection conn = GetConnection())
            {   
                //We use formatNullableString for non nullable strings so that
                //we don't accidently insert an empty string and instead cause an SQL exception
                string query = $"INSERT INTO {DatabaseUserCameraAssociation.TABLE_NAME}(" +
                               $"{DatabaseUserCameraAssociation.CAMERA_ID}," +
                               $"{DatabaseUserCameraAssociation.USER_ID}" +
                               ") VALUES (" +
                               $"{cameraId}, " +
                               $"{userId});";

                conn.Open();     
                MySqlCommand cmd = new MySqlCommand(query, conn);

                int success = cmd.ExecuteNonQuery();
                if (success != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public List<DatabaseUserCameraAssociation> GetAllUserCameraAssociations()
        {
            List<DatabaseUserCameraAssociation> userList = new List<DatabaseUserCameraAssociation>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseUserCameraAssociation.TABLE_NAME}";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseUserCameraAssociation user = new DatabaseUserCameraAssociation
                        {
                            UserId = Convert.ToInt32(reader[DatabaseUserCameraAssociation.USER_ID]),
                            CameraId = Convert.ToInt32(reader[DatabaseUserCameraAssociation.CAMERA_ID])
                        };
                        userList.Add(user);
                    }
                }
            }
            return userList;
        }

    }
}
