using System.Collections.Generic;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractLocationService
    {
        LocationInformationList GetAvailableLocationsForUser(int userId);
        bool SaveLocation(LocationDetails locationDetails);
        LocationInformationList GetAvailableLocations();
        List<RoomInfo> GetRoomsAtLocation(int locationId);
    }
}
