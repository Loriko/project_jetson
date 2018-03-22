using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Services;

namespace BackEndServer.Models.DBModels
{
    public class DatabasePerHourStats
    {
        // Database version of this object is the same as the defined object, but with a database context.
        private DatabaseQueryService _databaseQueryService;
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public double HourlyAverage { get; set; }
    }
}
