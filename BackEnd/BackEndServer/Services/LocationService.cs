using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services
{
    public class LocationService : AbstractLocationService
    {
        private readonly DatabaseQueryService _dbQueryService;

        public LocationService(DatabaseQueryService dbQueryService)
        {
            this._dbQueryService = dbQueryService;
        }

        
        public LocationInformationList getAvailableLocationsForUser(string username)
        {
            List<DatabaseAddress> dbAddressList = _dbQueryService.GetLocationsForUser(username);
            return new LocationInformationList(dbAddressList);
        }
    }
}