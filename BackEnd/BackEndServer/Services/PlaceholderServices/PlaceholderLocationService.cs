using System;
using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;

namespace BackEndServer.Services.PlaceholderServices
{
    public class PlaceholderLocationService : AbstractLocationService
    {
        public LocationInformationList getAvailableLocationsForUser(string username){
            List<LocationInformation> locationsAvailableForUser = new List<LocationInformation>();
            locationsAvailableForUser.Add(new LocationInformation(1, "DMV Merivale"));
            locationsAvailableForUser.Add(new LocationInformation(2, "DMV Ridgemont"));
            locationsAvailableForUser.Add(new LocationInformation(3, "DMV Walkley"));
            locationsAvailableForUser.Add(new LocationInformation(4, "DMV Westgate"));
            LocationInformationList locationInformationList = new LocationInformationList();
            locationInformationList.LocationList = locationsAvailableForUser;
            return locationInformationList;
        }
    }
}
