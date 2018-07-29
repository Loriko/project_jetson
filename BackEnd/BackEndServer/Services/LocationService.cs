using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services
{
    public class LocationService : AbstractLocationService
    {
        private readonly IDatabaseQueryService _dbQueryService;

        public LocationService(IDatabaseQueryService dbQueryService)
        {
            _dbQueryService = dbQueryService;
        }

        
        public LocationInformationList getAvailableLocationsForUser(string username)
        {
            List<DatabaseLocation> dbAddressList = _dbQueryService.GetLocationsForUser(username);
            return new LocationInformationList(dbAddressList);
        }

        public bool SaveLocation(LocationDetails locationDetails)
        {
            DatabaseLocation dbLocation = new DatabaseLocation(locationDetails);
            return _dbQueryService.PersistNewLocation(dbLocation);
        }
    }
}