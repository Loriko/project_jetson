using System;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseLocation
    {
        // Table Name
        public static readonly string TABLE_NAME = "location";
        // Attributes of Location table.
        public static readonly string LOCATION_ID_LABEL = "id";
        public static readonly string LOCATION_NAME_LABEL = "location_name";
        public static readonly string ADDRESS_LINE_LABEL = "address_line";
        public static readonly string CITY_LABEL = "city";
        public static readonly string STATE_LABEL = "state";
        public static readonly string ZIP_LABEL = "zip";

        // Database Model Class Attributes
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}
