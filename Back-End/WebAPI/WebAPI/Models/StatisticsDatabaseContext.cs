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

        // The following methods are to be used by the Web API controllers.

        #region Methods to retrieve data from the StatisticsDatabase

        /// <summary>
        /// Queries all PerSecondStats objects in the StatisticsDatabase, groups them in a single DataMessage which will be serialized to JSON and returned to Front-End Clients.
        /// </summary>
        /// <param name="verifiedTimeInterval">Verification of the TimeInterval must be performed by the controller before calling this method.</param>
        /// <returns></returns>
        public DataMessage getStatsFromInterval(TimeInterval verifiedTimeInterval)
        {
            // Query the MySQL database using the interval, group results in an array of PerSecondStats, create a Datamessage object and return it.


            // will be removed.
            return (null);
        }

        #endregion

        #region Methods to store data into the StatisticsDatabase

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

        #endregion
    }
}
