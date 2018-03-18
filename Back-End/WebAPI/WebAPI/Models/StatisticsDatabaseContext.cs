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
                // I finished this one without the query, so you know how to complete the other ones.

                // FRANCIS*****************************************************************
                string query = "";
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStatsList.Add(new DatabasePerSecondStats()
                        {
                            // FRANCIS **********************************************************************************************************
                            // The strings within the reader[] section must match the column names that SARMAD specified in the My SQL schema.

                            UnixTime = MySqlDateTimeConverter.toDateTime(Convert.ToString(reader["Datetime ???"])).toUnixTime(),
                            CameraID = Convert.ToInt32(reader["CameraID ???"]),
                            NumTrackedPeople = Convert.ToInt32(reader["NumTrackedPeople ???"]),
                            HasSavedImage = Convert.ToBoolean(reader["HasSavedImage ???"])
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
            if (dataMessage.RealTimeStats == null || dataMessage.RealTimeStats.Length < 1)
                return (false);

            int length = dataMessage.RealTimeStats.Length;

            for (int x = 0; x < length; x++)
            {
                try
                {
                    // Store element x from array into the database.

                    // use this.StoreSQL or similar
                }
                catch(Exception e)
                {
                    return (false);
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
            //AveragesOfDayResponse averagesOfDayResponse = new AveragesOfDayResponse();

            // Query the 24 Hourly Averages in the database.

            // If result does not contain 24 rows, then recalculate all of them and store the averages in the database, at the same time place them in the return object. 
            // If it does, put them in the averagesOfDayResponse object and return.

            for (int z = 0; z < 24; z++)
            {

            }

            return (null);
        }

        public PerSecondStats getSpecificSecond(SingleSecondTime singleSecondTime)
        {


            return (null);
        }

        public Camera[] getCamerasForLocation(int locationId)
        {




            return (null);
        }
    }
}