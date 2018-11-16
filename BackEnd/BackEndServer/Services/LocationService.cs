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

        public LocationDetailsList GetLocationsCreatedByUser(int userId)
        {
            List<DatabaseLocation> dbAddressList = _dbQueryService.GetLocationsCreatedByUser(userId);
            return new LocationDetailsList(dbAddressList);
        }

        public bool SaveLocation(LocationDetails locationDetails)
        {
            DatabaseLocation dbLocation = new DatabaseLocation(locationDetails);
            if (dbLocation.LocationId > 0)
            {
                return _dbQueryService.PersistExistingLocation(dbLocation);
            }
            else
            {
                return _dbQueryService.PersistNewLocation(dbLocation);
            }
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

        public bool ValidateNewRoomName(int locationId, string roomName)
        {
            return _dbQueryService.GetRoomIdByLocationIdAndRoomName(locationId, roomName) <= 0;
        }

        //Will only work if all associated cameras have been unclaimed/deleted
        public bool DeleteLocation(int locationId)
        {
            if (_dbQueryService.DeleteRoomsAtLocation(locationId))
            {
                return _dbQueryService.DeleteLocation(locationId);
            }

            return false;
        }

        public bool ValidateNewLocationName(string locationName, int userId)
        {
            List<DatabaseLocation> userLocations = _dbQueryService.GetLocationsCreatedByUser(userId);
            return userLocations.TrueForAll(location => location.LocationName.ToUpper() != locationName.ToUpper());
        }
    }
}