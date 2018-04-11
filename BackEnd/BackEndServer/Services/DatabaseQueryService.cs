using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using BackEndServer.Models.DBModels;
using BackEndServer.Classes.DataRequestClasses;
using BackEndServer.Classes.DataResponseClasses;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Services.HelperServices;

// More Info: http://www.c-sharpcorner.com/article/how-to-connect-mysql-with-asp-net-core/

// Bulk insert: INSERT INTO tbl_name (a,b,c) VALUES(1,2,3),(4,5,6),(7,8,9);

namespace BackEndServer.Services
{   
    
    public class DatabaseQueryService
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

        // Untested -- Mohamed R.
        public bool storePerSecondStats(List<PerSecondStat> distinctStats)
        {
            string bulkInsertCommand = "INSERT INTO perSecondStat (dateTime,Camera_idCamera,numDetectedObjects,hasSavedImage) VALUES ";

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                PerSecondStat lastStat = distinctStats.Last();

                // Build Bulk Insert Command
                foreach (PerSecondStat stat in distinctStats)
                {
                    string camId = stat.CameraId.ToString();
                    string numPeople = stat.NumTrackedPeople.ToString();
                    string hasImage = "0";

                    if (stat.HasSavedImage)
                    {
                        hasImage = "1";
                    }

                    bulkInsertCommand += $"(\"{stat.DateTime}\",{camId},{numPeople},{hasImage})";

                    if (stat != lastStat)
                    {
                        bulkInsertCommand += ",";
                    }
                }
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

        //Right now, username isn't used, but it will be as soon as the tables necessary for camera permissions are added
        public List<DatabaseAddress> GetLocationsForUser(string username)
        {
            List<DatabaseAddress> locationList = new List<DatabaseAddress>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = "select * from address";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        locationList.Add(new DatabaseAddress()
                        {
                            idAddress = Convert.ToInt32(reader["idAddress"]),
                            location = Convert.ToString(reader["location"])
                        });
                    }
                }

            }
            return locationList;
        }
        
        // Return DatabasePerSecondStat, not PerSecondStat, this is faster to implement for a simple test.
        public List<TestObject> testDatabase()
        {
            List<TestObject> perSecondStatsList = new List<TestObject>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = "select * from test";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStatsList.Add(new TestObject()
                        {
                            CameraID = Convert.ToInt32(reader["idtest"])
                        });
                    }
                }

            }
            return (perSecondStatsList);
        }

        // Untested -- Mohamed R.
        public List<DatabasePerSecondStat> getStatsFromInterval(TimeInterval verifiedTimeInterval)
        {
            List<DatabasePerSecondStat> perSecondStatsList = new List<DatabasePerSecondStat>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = "SELECT * FROM perSecondStat WHERE dateTime >= " + verifiedTimeInterval.StartDateTime;
                query += " AND dateTime <= " + verifiedTimeInterval.EndDateTime;

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStatsList.Add(new DatabasePerSecondStat()
                        {
                            DateTime = Convert.ToDateTime(reader["dateTime"]),
                            CameraId = Convert.ToInt32(reader["Camera_idCamera"]),
                            NumDetectedObjects = Convert.ToInt32(reader["numDetectedObjects"]),
                            HasSavedImage = Convert.ToBoolean(Convert.ToInt16(reader["hasSavedImage"]))
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
                string query = $"SELECT * from perSecondStat WHERE Camera_idCamera = {cameraId} ORDER BY dateTime DESC LIMIT 1;";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    // Expecting only one row
                    if (reader.Read())
                    {
                        perSecondStat = new DatabasePerSecondStat()
                        {
                            CameraId = Convert.ToInt32(reader["Camera_idCamera"]),
                            HasSavedImage = Convert.ToBoolean(reader["hasSavedImage"]),
                            DateTime = Convert.ToDateTime(reader["dateTime"]),
                            NumDetectedObjects = Convert.ToInt32(reader["numDetectedObjects"]),
                            StatId = Convert.ToInt32(reader["idStat"])
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
                string query = $"SELECT * from camera WHERE idCamera = {cameraId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        camera = new DatabaseCamera()
                        {
                            CameraId = Convert.ToInt32(reader["idCamera"]),
                            CameraName = Convert.ToString(reader["cameraName"]),
                            LocationId = Convert.ToInt32(reader["Address_idAddress"]),
                            UserID = Convert.ToInt32(reader["User_idUser"]),
                            MonitoredArea = Convert.ToString(reader["roomName"])
                        };
                    }
                }

            }
            return camera;
        }
        
        public List<DatabasePerSecondStat> GetPerSecondStatsForCamera(int cameraId)
        {
            List<DatabasePerSecondStat> perSecondStats = new List<DatabasePerSecondStat>();

            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * from {DatabasePerSecondStat.TABLE_NAME} WHERE {DatabasePerSecondStat.CAMERA_ID_LABEL} = {cameraId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStats.Add(new DatabasePerSecondStat()
                        {
                            StatId = Convert.ToInt32(reader[DatabasePerSecondStat.STAT_ID_LABEL]),
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
                string query = $"SELECT * from camera WHERE Address_idAddress = {locationId};";
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cameraList.Add(new DatabaseCamera()
                        {
                            CameraId = Convert.ToInt32(reader["idCamera"]),
                            CameraName = Convert.ToString(reader["cameraName"]),
                            LocationId = Convert.ToInt32(reader["Address_idAddress"]),
                            UserID = Convert.ToInt32(reader["User_idUser"]),
                            MonitoredArea = Convert.ToString(reader["roomName"])
                        });
                    }
                }

            }
            return cameraList;
        }
        
        public bool IsPasswordValidForUser(string username, string password)
        {
            using (MySqlConnection conn = GetConnection())
            {
                string query = $"SELECT * from User WHERE username = \"{username}\" AND password = \"{password}\" LIMIT 1;";
                
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="averagesOfDayRequest"></param>
        /// <returns></returns>
        /*public AveragesOfDayResponse getHourlyAveragesForDay(AveragesOfDayRequest averagesOfDayRequest)
        {
            //AveragesOfDayResponse averagesOfDayResponse = new AveragesOfDayResponse();

            // Query the 24 Hourly Averages in the database.

            // If result does not contain 24 rows, then recalculate all of them and store the averages in the database, at the same time place them in the return object. 
            // If it does, put them in the averagesOfDayResponse object and return.

            for (int z = 0; z < 24; z++)
            {

            }
            return (null);
        }*/
    }
}