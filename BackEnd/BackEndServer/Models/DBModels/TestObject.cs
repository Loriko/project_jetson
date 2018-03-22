using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Services;

namespace BackEndServer.Models.DBModels
{
    public class TestObject
    {
        // Database version of this object is the same as the defined object, but with a database context.
        private DatabaseQueryService _databaseQueryService;
        public DateTime DateTime { get; set; }
        public int CameraID { get; set; }
        public int NumTrackedPeople { get; set; }
        public bool HasSavedImage { get; set; }
    }
}
