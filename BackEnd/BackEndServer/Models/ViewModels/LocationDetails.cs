using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class LocationDetails
    {
        public int LocationId { get; set; }
        public int UserId { get; set; }
        public string LocationName { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public LocationDetails()
        {
        }
        
        public LocationDetails(DatabaseLocation databaseAddress)
        {
            LocationId = databaseAddress.LocationId;
            UserId = databaseAddress.UserId;
            LocationName = databaseAddress.LocationName;
            AddressLine = databaseAddress.AddressLine;
            City = databaseAddress.City;
            State = databaseAddress.State;
            Zip = databaseAddress.Zip;
        }
    }
}