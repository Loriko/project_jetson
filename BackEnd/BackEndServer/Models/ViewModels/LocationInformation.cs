using System;
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
        public LocationInformation()
        {
        }
    }
}
