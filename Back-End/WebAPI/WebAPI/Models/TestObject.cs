using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class TestObject
    {
        // Database version of this object is the same as the defined object, but with a database context.
        private StatisticsDatabaseContext databaseContext;
        public DateTime DateTime { get; set; }
        public int CameraID { get; set; }
        public int NumTrackedPeople { get; set; }
        public bool HasSavedImage { get; set; }
    }
}
