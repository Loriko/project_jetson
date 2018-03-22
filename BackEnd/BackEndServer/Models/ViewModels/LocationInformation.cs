using System;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class LocationInformation
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }

        public LocationInformation(int locationId, string locationName)
        {
            this.LocationId = locationId;
            this.LocationName = locationName;
        }
        // Adapter for LocationInformation constructor out of a DatabaseAddress object
        public LocationInformation(DatabaseAddress databaseAddress) : this(databaseAddress.idAddress, databaseAddress.location){}
    }
}
