using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services
{
    public class LocationService : AbstractLocationService
    {
        public LocationInformationList getAvailableLocationsForUser(string username)
        {
            DatabaseQueryService dbQueryService = new DatabaseQueryService();
            List<DatabaseAddress> dbAddressList = dbQueryService.GetLocationsForUser(username);
            return new LocationInformationList(dbAddressList);
        }
    }
}