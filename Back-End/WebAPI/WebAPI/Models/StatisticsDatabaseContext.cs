using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using WebAPI.Object_Classes;
using WebAPI.Data_Request_Classes;
using WebAPI.Data_Response_Classes;
using WebAPI.Helper_Classes;

// More Info: http://www.c-sharpcorner.com/article/how-to-connect-mysql-with-asp-net-core/

namespace WebAPI.Models
{
    public class StatisticsDatabaseContext
    {
        #region Database Context
        // Connection String Attribute
        public string ConnectionString { get; set; }

        // Constructor
        public StatisticsDatabaseContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        // Method to return a MySQL Database connection.
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
        #endregion

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
        /// </summary>
        /// <param name="dataMessage"></param>
        /// <returns>True if persist operations were successful. False if something is wrong. Returns a boolean to inform controller of what response code to return to the HTTP client.</returns>
        public bool storeStatsFromMessage(DataMessage dataMessage)
        {
            if (dataMessage.RealTimeStats == null || dataMessage.getLength() < 1)
                return (false);

            int initialLength = dataMessage.getLength();
            List<PerSecondStats> temp = new List<PerSecondStats>();

            for (int y = 0; y < initialLength; y++)
            {
                temp.Add(dataMessage.RealTimeStats[y]);
            }

            // Remove any possible duplicates.
            List<PerSecondStats> distinctListOfSecondStats = temp.Distinct().ToList();

            using (MySqlConnection conn = GetConnection())
            {
                foreach (PerSecondStats second in distinctListOfSecondStats)
                {
                    string mySqlDateTime = second.UnixTime.toDateTime().toMySqlDateTime();
                    string camId = second.CameraId.ToString();
                    string numPeople = second.NumTrackedPeople.ToString();
                    string hasImage = "0";

                    if (second.HasSavedImage == true)
                    {
                        hasImage = "1";
                    }

                    try
                    {
                        string insertCommand = "insert into perSecondStat (dateTime,Camera_idCamera,numDetectedObjects,hasSavedImage) ";
                        insertCommand += "values (" + mySqlDateTime + "," + camId + "," + numPeople + "," + hasImage + ")";

                        MySqlCommand cmd = new MySqlCommand(insertCommand, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        // write to log

                        return (false);
                    }
                }

            }

            return (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="averagesOfDayRequest"></param>
        /// <returns></returns>
        public AveragesOfDayResponse getHourlyAveragesForDay(AveragesOfDayRequest averagesOfDayRequest)
        {
            // Will be implemented by me.
            
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

        public Camera[] getCamerasForLocation(int locationId)
        {
            // Will be implemented by me soon.

            return (null);
        }
    }
}