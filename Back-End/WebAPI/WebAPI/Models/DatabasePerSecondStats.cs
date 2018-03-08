using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Object_Classes;

// More info: http://www.c-sharpcorner.com/article/how-to-connect-mysql-with-asp-net-core/

namespace WebAPI.Models
{
    public class DatabasePerSecondStats
    {
        // Database version of this object is the same as the defined object, but with a database context.
        private StatisticsDatabaseContext databaseContext;
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int CameraID { get; set; }
        public int NumTrackedPeople { get; set; }
        public bool HasSavedImage { get; set; }
    }
}
