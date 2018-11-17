using System;
using System.Collections.Generic;
using BackEndServer.Models.DBModels;


namespace BackEndServer.Models.ViewModels
{
    public class LocationDetailsList
    {
        public List<LocationDetails> LocationList { get; set; }
        
        public LocationDetailsList()
        {
            LocationList = new List<LocationDetails>();
        }

        public LocationDetailsList(List<DatabaseLocation> dbAddressList)
        {
            LocationList = new List<LocationDetails>();
            foreach (var databaseAddress in dbAddressList)
            {
                LocationList.Add(new LocationDetails(databaseAddress));
            }
        }
    }
}
