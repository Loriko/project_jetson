using BackEndServer.Models.ViewModels;
using MySql.Data.MySqlClient;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseLocation
    {
        // Table Name
        public static readonly string TABLE_NAME = "location";
        // Attributes of Location table.
        public static readonly string LOCATION_ID_LABEL = "id";
        public static readonly string USER_ID_LABEL = "user_id";
        public static readonly string LOCATION_NAME_LABEL = "location_name";
        public static readonly string ADDRESS_LINE_LABEL = "address_line";
        public static readonly string CITY_LABEL = "city";
        public static readonly string STATE_LABEL = "state";
        public static readonly string ZIP_LABEL = "zip";

        // Database Model Class Attributes
        public int LocationId { get; set; }
        public int UserId { get; set; }
        public string LocationName { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public DatabaseLocation()
        {
        }

        public DatabaseLocation(LocationDetails locationDetails)
        {
            LocationId = locationDetails.LocationId;
            UserId = locationDetails.UserId;
            LocationName = locationDetails.LocationName;
            AddressLine = locationDetails.AddressLine;
            City = locationDetails.City;
            State = locationDetails.State;
            Zip = locationDetails.Zip;
        }
        
        public void EscapeStringFields()
        {
            if (LocationName != null)
                LocationName = MySqlHelper.EscapeString(LocationName);
            if (AddressLine != null)
                AddressLine = MySqlHelper.EscapeString(AddressLine);
            if (City != null)
                City = MySqlHelper.EscapeString(City);
            if (State != null)
                State = MySqlHelper.EscapeString(State);
            if (Zip != null)
                Zip = MySqlHelper.EscapeString(Zip);
        }
    }
}