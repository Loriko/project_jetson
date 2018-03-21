using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Services;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseAddress
    {
        private DatabaseQueryService _databaseQueryService;
        public int idAddress { get; set; }
        public string location { get; set; }
    }
}
