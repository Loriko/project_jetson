using System.Collections.Generic;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractLocationService
    {
        LocationInformationList GetAvailableLocationsForUser(int userId);
        LocationDetailsList GetLocationsCreatedByUser(int userId);
        bool SaveLocation(LocationDetails locationDetails);
        LocationInformationList GetAvailableLocations();
        List<RoomInfo> GetRoomsAtLocation(int locationId);
        bool ValidateNewRoomName(int locationid, string roomName);
        bool DeleteLocation(int locationId);
        bool ValidateNewLocationName(string locationName, int userId);
    }
}
