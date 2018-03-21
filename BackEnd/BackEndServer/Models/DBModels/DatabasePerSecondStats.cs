using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Services;

// More info: http://www.c-sharpcorner.com/article/how-to-connect-mysql-with-asp-net-core/

namespace BackEndServer.Models.DBModels
{
    public class DatabasePerSecondStats
    {
        // Database version of this object is the same as the defined object, but with a database context.
        private DatabaseQueryService _databaseQueryService;
        public long UnixTime { get; set; }
        public int CameraID { get; set; }
        public int NumTrackedPeople { get; set; }
        public bool HasSavedImage { get; set; }
    }
}
