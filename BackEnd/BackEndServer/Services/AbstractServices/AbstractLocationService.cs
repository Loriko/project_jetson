using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractLocationService
    {
        LocationInformationList getAvailableLocationsForUser(int userId);
        bool SaveLocation(LocationDetails locationDetails);
    }
}
