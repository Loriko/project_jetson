using System.Collections.Generic;

namespace BackEndServer.Models.ViewModels
{
    public class RoomSelectorInfo
    {
        public List<RoomInfo> Rooms { get; set; }
        public int LocationId { get; set; }
        public string SelectorId { get; set; }
        public string SelectorName { get; set; }
        public int? SelectedRoomId { get; set; }
        public bool Required { get; set; }
        
        public RoomSelectorInfo(int locationId, string selectorId,
            string selectorName, bool required, int? selectedLocationId = null)
        {
            LocationId = locationId;
            SelectorId = selectorId;
            SelectorName = selectorName;
            Required = required;
            SelectedRoomId = selectedLocationId;
        }
        
        public RoomSelectorInfo(List<RoomInfo> rooms, int locationId, string selectorId,
            string selectorName, bool required, int? selectedLocationId = null)
        {
            Rooms = rooms;
            LocationId = locationId;
            SelectorId = selectorId;
            SelectorName = selectorName;
            Required = required;
            SelectedRoomId = selectedLocationId;
        }
        
        public RoomSelectorInfo()
        {
        }
    }
}