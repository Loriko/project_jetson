namespace BackEndServer.Models.ViewModels
{
    public class LocationSelectorInfo
    {
        public LocationInformationList Locations { get; set; }
        public string SelectorId;
        public string SelectorName;
        public int? SelectedLocationId;

        public LocationSelectorInfo(string selectorId,
            string selectorName, int? selectedLocationId = null)
        {
            SelectorId = selectorId;
            SelectorName = selectorName;
            SelectedLocationId = selectedLocationId;
        }
        
        public LocationSelectorInfo(LocationInformationList locations, string selectorId,
                                  string selectorName, int? selectedLocationId = null)
        {
            Locations = locations;
            SelectorId = selectorId;
            SelectorName = selectorName;
            SelectedLocationId = selectedLocationId;
        }
        
        public LocationSelectorInfo()
        {
        }
    }
}