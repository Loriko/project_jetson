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

        // Return DatabasePerSecondStats, not PerSecondStats, this is faster to implement for a simple test.
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
        /// Queries all PerSecondStats objects in the StatisticsDatabase, groups them in a single DataMessage which will be serialized by the to JSON by the controller and returned to Front-End Clients.
        /// </summary>
        /// <param name="verifiedTimeInterval">Verification of the TimeInterval must be performed by the controller before calling this method.</param>
        /// <returns>All requested PerSecondStats within the specified interval, grouped in a DataMessage object.</returns>
        public DataMessage getStatsFromInterval(TimeInterval verifiedTimeInterval)
        {
            List<DatabasePerSecondStats> perSecondStatsList = new List<DatabasePerSecondStats>();

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
                        perSecondStatsList.Add(new DatabasePerSecondStats()
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
            PerSecondStats[] temp = new PerSecondStats[numQueryResults];

            foreach (DatabasePerSecondStats second in perSecondStatsList)
            {
                temp[i] = new PerSecondStats(second.CameraID, second.UnixTime, second.NumTrackedPeople, second.HasSavedImage);
                i++;
            }

            DataMessage responseDataMessage = new DataMessage(temp);

            return (responseDataMessage);
        }

        /// <summary>
        /// Stores every individual PerSecondStat object from a DataMessage (received from Capture System) into the StatisticsDatabase.
        /// IMPORTANT: Last version used unix timestamps. Francis changed this to use the old persecondstat because of time constraints
        /// TODO: REFACTOR THIS ASAP. RIGHT NOW A QUERY IS STARTED FOR EACH PER SECOND STAT INSTEAD OF JUST INSERTING ALL OF THEM IN THE SAME QUERY 
        /// </summary>
        /// <param name="dataMessage"></param>
        /// <returns>True if persist operations were successful. False if something is wrong. Returns a boolean to inform controller of what response code to return to the HTTP client.</returns>
        public bool storeStatsFromMessage(OldDataMessage dataMessage)
        {
            if (dataMessage.RealTimeStats == null || dataMessage.getLength() < 1)
                return false;

            int initialLength = dataMessage.getLength();
            List<OldPerSecondStat> temp = new List<OldPerSecondStat>();

            for (int y = 0; y < initialLength; y++)
            {
                temp.Add(dataMessage.RealTimeStats[y]);
            }

            // Remove any possible duplicates.
            List<OldPerSecondStat> distinctListOfSecondStats = temp.Distinct().ToList();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                foreach (OldPerSecondStat perSecondStat in distinctListOfSecondStats)
                {
                    string mySqlDateTime = new DateTime(perSecondStat.Year, perSecondStat.Month, perSecondStat.Day, perSecondStat.Hour, perSecondStat.Minute, perSecondStat.Second).toMySqlDateTime();
                    string camId = perSecondStat.CameraId.ToString();
                    string numPeople = perSecondStat.NumTrackedPeople.ToString();
                    string hasImage = "0";

                    if (perSecondStat.HasSavedImage)
                    {
                        hasImage = "1";
                    }

                    try
                    {
                        string insertCommand = "insert into perSecondStat (dateTime,Camera_idCamera,numDetectedObjects,hasSavedImage) ";
                        insertCommand += $"values (\"{mySqlDateTime}\",{camId},{numPeople},{hasImage})";
                        MySqlCommand cmd = new MySqlCommand(insertCommand, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        // write to log
                        return false;
                    }
                }
            }
            return true;
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
        /// <returns>Null if nothing was found in the database. A PerSecondStats object if found.</returns>
        public PerSecondStats getSpecificSecond(SingleSecondTime singleSecondTime)
        {
            List<DatabasePerSecondStats> perSecondStatsList = new List<DatabasePerSecondStats>();

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
                        perSecondStatsList.Add(new DatabasePerSecondStats()
                        {
                            UnixTime = MySqlDateTimeConverter.toDateTime(Convert.ToString(reader["dateTime"])).toUnixTime(),
                            CameraID = Convert.ToInt32(reader["Camera_idCamera"]),
                            NumTrackedPeople = Convert.ToInt32(reader["numDetectedObjects"]),
                            HasSavedImage = Convert.ToBoolean(Convert.ToInt16(reader["hasSavedImage"]))
                        });
                    }
                }
            }

            DatabasePerSecondStats temp = perSecondStatsList.ElementAt(0);
            PerSecondStats result = null;
            
            // If a result was found by the query.
            if (temp != null)
                result = new PerSecondStats(temp.CameraID, temp.UnixTime, temp.NumTrackedPeople, temp.HasSavedImage);

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