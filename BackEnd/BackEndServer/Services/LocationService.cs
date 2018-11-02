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

        
        public LocationInformationList GetAvailableLocationsForUser(int userId)
        {
            List<DatabaseLocation> dbAddressList = _dbQueryService.GetLocationsForUser(userId);
            return new LocationInformationList(dbAddressList);
        }

        public bool SaveLocation(LocationDetails locationDetails)
        {
            DatabaseLocation dbLocation = new DatabaseLocation(locationDetails);
            return _dbQueryService.PersistNewLocation(dbLocation);
        }

        public LocationInformationList GetAvailableLocations()
        {
            List<DatabaseLocation> dbAddressList = _dbQueryService.GetLocations();
            return new LocationInformationList(dbAddressList);
        }

        public List<RoomInfo> GetRoomsAtLocation(int locationId)
        {
            List<DatabaseRoom> dbRoomList = _dbQueryService.GetRoomsAtLocation(locationId);
            List<RoomInfo> roomList = new List<RoomInfo>();
            foreach (var dbRoom in dbRoomList)
            {
                roomList.Add(new RoomInfo(dbRoom));
            }
            return roomList;
        }
    }
}