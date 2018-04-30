using System;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;


namespace BackEndServer.Models.ViewModels
{
    public class LocationInformationList
    {
        public List<LocationInformation> LocationList { get; set; }
        
        public LocationInformationList()
        {
            LocationList = new List<LocationInformation>();
        }

        public LocationInformationList(List<DatabaseLocation> dbAddressList)
        {
            LocationList = new List<LocationInformation>();
            foreach (var databaseAddress in dbAddressList)
            {
                LocationList.Add(new LocationInformation(databaseAddress));
            }
        }
    }
}
