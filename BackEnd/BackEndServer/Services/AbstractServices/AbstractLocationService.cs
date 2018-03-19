using System;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractLocationService
    {
        LocationInformationList getAvailableLocationsForUser(string username);
    }
}
