﻿using System;
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

        // Quick hack because I can't figure out dependency injection right now
        // TODO: Remove this constructor and figure out how this service is to be injected into other services
        public DatabaseQueryService()
        {
            this.ConnectionString = "server=localhost;port=3306;database=mydb;user=root;password=password;SslMode=none";
        }
        
        // Method to return a MySQL Database connection.
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
        #endregion

        public bool storePerSecondStat(List<PerSecondStat> distinctStats)
        {
            string bulkInsertCommand = "insert into perSecondStat (dateTime,Camera_idCamera,numDetectedObjects,hasSavedImage) values ";

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
        
        /* The following methods are to be used by the Web API controllers. */

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

        /// <summary>
        /// Queries all PerSecondStat objects in the StatisticsDatabase, groups them in a single DataMessage which will be serialized by the to JSON by the controller and returned to Front-End Clients.
        /// </summary>
        /// <param name="verifiedTimeInterval">Verification of the TimeInterval must be performed by the controller before calling this method.</param>
        /// <returns>All requested PerSecondStat within the specified interval, grouped in a DataMessage object.</returns>
        public DataMessage getStatsFromInterval(TimeInterval verifiedTimeInterval)
        {
            List<DatabasePerSecondStat> perSecondStatsList = new List<DatabasePerSecondStat>();

            using (MySqlConnection conn = GetConnection())
            {
                string lowerBoundMySqlTime = verifiedTimeInterval.StartUnixTime.toDateTime().toMySqlDateTime();
                string upperBoundMySqlTime = verifiedTimeInterval.EndUnixTime.toDateTime().toMySqlDateTime();

                string query = "select * from perSecondStat where dateTime >= " + lowerBoundMySqlTime;
                query += " and dateTime <= " + upperBoundMySqlTime;

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStatsList.Add(new DatabasePerSecondStat()
                        {
                            UnixTime = MySqlDateTimeConverter.toDateTime(Convert.ToString(reader["dateTime"])).toUnixTime(),
                            CameraID = Convert.ToInt32(reader["Camera_idCamera"]),
                            NumTrackedPeople = Convert.ToInt32(reader["numDetectedObjects"]),
                            HasSavedImage = Convert.ToBoolean(Convert.ToInt16(reader["hasSavedImage"]))
                        });
                    }
                }

            }

            int numQueryResults = perSecondStatsList.Count;
            int i = 0;
            PerSecondStat[] temp = new PerSecondStat[numQueryResults];

            foreach (DatabasePerSecondStat second in perSecondStatsList)
            {
                temp[i] = new PerSecondStat(second.CameraID, second.UnixTime, second.NumTrackedPeople, second.HasSavedImage);
                i++;
            }

            DataMessage responseDataMessage = new DataMessage(temp);

            return (responseDataMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="averagesOfDayRequest"></param>
        /// <returns></returns>
        public AveragesOfDayResponse getHourlyAveragesForDay(AveragesOfDayRequest averagesOfDayRequest)
        {
            //AveragesOfDayResponse averagesOfDayResponse = new AveragesOfDayResponse();

            // Query the 24 Hourly Averages in the database.

            // If result does not contain 24 rows, then recalculate all of them and store the averages in the database, at the same time place them in the return object. 
            // If it does, put them in the averagesOfDayResponse object and return.

            for (int z = 0; z < 24; z++)
            {

            }
            return (null);
        }

        /// <summary>
        /// Service to obtain the statistics for a specified single second.
        /// </summary>
        /// <param name="singleSecondTime"></param>
        /// <returns>Null if nothing was found in the database. A PerSecondStat object if found.</returns>
        public PerSecondStat getSpecificSecond(SingleSecondTime singleSecondTime)
        {
            List<DatabasePerSecondStat> perSecondStatsList = new List<DatabasePerSecondStat>();

            using (MySqlConnection conn = GetConnection())
            {
                string timeToObtain = singleSecondTime.UnixTime.toDateTime().toMySqlDateTime();
                string query = "select * from perSecondStat where dateTime = " + timeToObtain;
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStatsList.Add(new DatabasePerSecondStat()
                        {
                            UnixTime = MySqlDateTimeConverter.toDateTime(Convert.ToString(reader["dateTime"])).toUnixTime(),
                            CameraID = Convert.ToInt32(reader["Camera_idCamera"]),
                            NumTrackedPeople = Convert.ToInt32(reader["numDetectedObjects"]),
                            HasSavedImage = Convert.ToBoolean(Convert.ToInt16(reader["hasSavedImage"]))
                        });
                    }
                }
            }

            DatabasePerSecondStat temp = perSecondStatsList.ElementAt(0);
            PerSecondStat result = null;
            
            // If a result was found by the query.
            if (temp != null)
                result = new PerSecondStat(temp.CameraID, temp.UnixTime, temp.NumTrackedPeople, temp.HasSavedImage);

            return (result);
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
    }
}