using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using WebAPI.Object_Classes;
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
                // Stuck on query, not sure how I will be able to query between year, month, day, hour, minute, second...

                string query = "";
                /*
                if ()
                {

                }
                else if ()
                {

                }
                */

                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        perSecondStatsList.Add(new DatabasePerSecondStats()
                        {
                            Year = Convert.ToInt32(reader["Year"]),
                            Month = Convert.ToInt32(reader["Month"]),
                            Day = Convert.ToInt32(reader["Day"]),
                            Hour = Convert.ToInt32(reader["Hour"]),
                            Minute = Convert.ToInt32(reader["Minute"]),
                            Second = Convert.ToInt32(reader["Second"]),
                            CameraID = Convert.ToInt32(reader["CameraID"]),
                            NumTrackedPeople = Convert.ToInt32(reader["NumTrackedPeople"]),
                            HasSavedImage = Convert.ToBoolean(reader["HasSavedImage"])
                        });
                    }
                }

            }

            int numQueryResults = perSecondStatsList.Count;
            DataMessage responseDataMessage = new DataMessage(numQueryResults);
            int i = 0;

            foreach (DatabasePerSecondStats second in perSecondStatsList)
            {
                responseDataMessage.RealTimeStats[i] = new PerSecondStats(second.CameraID, second.Year, second.Month, second.Day, second.Hour, second.Minute, second.Second, second.NumTrackedPeople, second.HasSavedImage);
                i++;
            }

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
            AveragesOfDayResponse averagesOfDayResponse = new AveragesOfDayResponse();

            // Query the 24 Hourly Averages in the database.

            // If result does not contain 24 rows, then recalculate all of them and store the averages in the database, at the same time place them in the return object. 
            // If it does, put them in the averagesOfDayResponse object and return.

            for (int z = 0; z < 24; z++)
            {

            }

            return (averagesOfDayResponse);
        }

    }
}