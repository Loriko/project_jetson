using BackEndServer.Models.ViewModels;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseRoom
    {
        // Table Name
        public static readonly string TABLE_NAME = "room";
        // Attributes of Room table.
        public static readonly string ROOM_ID_LABEL = "id";
        public static readonly string LOCATION_ID_LABEL = "Location_id";
        public static readonly string ROOM_NAME_LABEL = "room_name";
        
        public int RoomId { get; set; }
        public int LocationId { get; set; }
        public string RoomName { get; set; }

        public DatabaseRoom()
        {
        }

        public DatabaseRoom(CameraDetails cameraDetails)
        {
            LocationId = cameraDetails.LocationId;
            RoomName = cameraDetails.NewRoomName;
        }
        
        public DatabaseRoom(RoomInfo roomInfo)
        {
            RoomId = roomInfo.RoomId;
            LocationId = roomInfo.LocationId;
            RoomName = roomInfo.RoomName;
        }
    }
}