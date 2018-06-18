using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using BackEndServer.Models.DBModels;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Services.HelperServices;
using BackEndServer.Models.APIModels;

// More Info: http://www.c-sharpcorner.com/article/how-to-connect-mysql-with-asp-net-core/
// Bulk insert: "INSERT INTO tbl_name (a,b,c) VALUES(1,2,3),(4,5,6),(7,8,9)"

namespace BackEndServer.Services
{   
    public class DatabaseQueryService : IDatabaseQueryService
    {   
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

        public bool PersistNewPerSecondStats(List<PerSecondStat> distinctStats)
        {
            // Define the bulk insert query without any values to insert.
            string bulkInsertCommand = $"INSERT INTO {DatabasePerSecondStat.TABLE_NAME} "
                + $"({DatabasePerSecondStat.CAMERA_ID_LABEL},{DatabasePerSecondStat.NUM_DETECTED_OBJECTS_LABEL}, "
                + $"{DatabasePerSecondStat.DATE_TIME_LABEL},{DatabasePerSecondStat.HAS_SAVED_IMAGE_LABEL}) VALUES ";

            // Append the values one by one to the bulk insert query.
            PerSecondStat lastStat = distinctStats.Last();

            foreach (PerSecondStat stat in distinctStats)
            {
                string cameraId = stat.CameraId.ToString();
                string numDetectedObjects = stat.NumTrackedPeople.ToString();
                string hasImage = "0";

                if (stat.HasSavedImage)
                {
                    hasImage = "1";
                }

                bulkInsertCommand += $"({cameraId},{numDetectedObjects},'{stat.DateTime}',{hasImage})";

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

        public List<DatabasePerSecondStat> GetAllSecondsForHour(DateTime dateTime)
        {
            string startDateTime = dateTime.GetHourBeginning().ToMySqlDateTimeString();
            string endDateTime = dateTime.GetHourEnd().ToMySqlDateTimeString();
            TimeInterval interval = new TimeInterval(startDateTime, endDateTime);
            return GetStatsFromInterval(interval);
        }

        // FRANCIS TO CHECK LATER 
        // Right now, username isn't used, but it will be as soon as the tables necessary for camera permissions are added.
        public List<DatabaseLocation> GetLocationsForUser(string username)
        {
            List<DatabaseLocation> locationList = new List<DatabaseLocation>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * FROM {DatabaseLocation.TABLE_NAME}";
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
                        camera = new DatabaseCamera()
                        {
                            CameraId = Convert.ToInt32(reader[DatabaseCamera.CAMERA_ID_LABEL]),
                            CameraName = Convert.ToString(reader[DatabaseCamera.CAMERA_NAME_LABEL]),
                            LocationId = Convert.ToInt32(reader[DatabaseCamera.LOCATION_ID_LABEL]),
                            UserId = Convert.ToInt32(reader[DatabaseCamera.USER_ID_LABEL]),
                            MonitoredArea = Convert.ToString(reader[DatabaseCamera.MONITORED_AREA_LABEL]),
                            Brand = Convert.ToString(reader[DatabaseCamera.BRAND_LABEL]),
                            Model = Convert.ToString(reader[DatabaseCamera.MODEL_LABEL])
                        };
                        if (reader[DatabaseCamera.RESOLUTION_LABEL] != DBNull.Value)
                        {
                            camera.Resolution = Convert.ToString(reader[DatabaseCamera.RESOLUTION_LABEL]);
                        }
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
                            HasSavedImage = Convert.ToBoolean(reader[DatabasePerSecondStat.HAS_SAVED_IMAGE_LABEL])
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
                        DatabaseCamera camera = new DatabaseCamera()
                        {
                            CameraId = Convert.ToInt32(reader[DatabaseCamera.CAMERA_ID_LABEL]),
                            CameraName = Convert.ToString(reader[DatabaseCamera.CAMERA_NAME_LABEL]),
                            LocationId = Convert.ToInt32(reader[DatabaseCamera.LOCATION_ID_LABEL]),
                            UserId = Convert.ToInt32(reader[DatabaseCamera.USER_ID_LABEL]),
                            MonitoredArea = Convert.ToString(reader[DatabaseCamera.MONITORED_AREA_LABEL]),
                            Brand = Convert.ToString(reader[DatabaseCamera.BRAND_LABEL]),
                            Model = Convert.ToString(reader[DatabaseCamera.MODEL_LABEL])
                        };
                        if (reader[DatabaseCamera.RESOLUTION_LABEL] != DBNull.Value)
                        {
                            camera.Resolution = Convert.ToString(reader[DatabaseCamera.RESOLUTION_LABEL]);
                        }
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
                string query = $"SELECT * FROM User WHERE username = '{username}' AND password = '{password}' LIMIT 1";
                
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
                string query = $"SELECT id FROM User WHERE username = '{username}';";
                
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
                string query = "INSERT INTO camera(" +
                               "camera_name, location_id, user_id, monitored_area, brand, model, resolution" +
                               ") VALUES " +
                               $"('{camera.CameraName}',{camera.LocationId},{camera.UserId}," +
                               $"'{camera.MonitoredArea}',{formatNullableString(camera.Brand)}," +
                               $"{formatNullableString(camera.Model)},{formatNullableString(camera.Resolution)});";
                
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

        public List<string> GetExistingCameraResolutions()
        {
            List<string> resolutionsInDB = new List<string>();
            using (MySqlConnection conn = GetConnection())
            {
                string query = "SELECT resolution FROM Camera WHERE resolution IS NOT NULL GROUP BY resolution ORDER BY COUNT(*) DESC;";
                
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

        #region API Key Related

        // Performs a SELECT to query the specified API Key in the database.
        public DatabaseAPIKey GetAPIKey(string unsalted_unhashed_api_key)
        {
            throw new NotImplementedException();
        }

        // Performs a SELECT to query all API Keys in the database.
        public List<DatabaseAPIKey> GetAllAPIKeys()
        {
            throw new NotImplementedException();
        }

        // Performs an INSERT to persist a newly create API Key (salted and hashed key value and its Salt value).
        public bool PersistAPIKey(APIKey api_key)
        {
            throw new NotImplementedException();
        }

        // Performs an UPDATE to set the specified API key in the database to inactive. 
        public bool DeactivateAPIKey(DatabaseAPIKey api_key)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}