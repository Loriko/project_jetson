using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class RoomInfo
    {
        public int RoomId { get; set; }
        public int LocationId { get; set; }
        public string RoomName { get; set; }

        public RoomInfo()
        {
        }

        public RoomInfo(DatabaseRoom dbRoom)
        {
            RoomId = dbRoom.RoomId;
            LocationId = dbRoom.LocationId;
            RoomName = dbRoom.RoomName;
        }
    }
}