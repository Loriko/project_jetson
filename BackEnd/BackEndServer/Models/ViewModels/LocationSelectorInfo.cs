namespace BackEndServer.Models.ViewModels
{
    public class LocationSelectorInfo
    {
        public LocationInformationList Locations { get; set; }
        public string SelectorId { get; set; }
        public string SelectorName { get; set; }
        public int? SelectedLocationId { get; set; }
        public bool Required { get; set; }
        
        public LocationSelectorInfo(string selectorId,
            string selectorName, bool required, int? selectedLocationId = null)
        {
            SelectorId = selectorId;
            SelectorName = selectorName;
            Required = required;
            SelectedLocationId = selectedLocationId;
        }
        
        public LocationSelectorInfo(LocationInformationList locations, string selectorId,
                                  string selectorName, bool required, int? selectedLocationId = null)
        {
            Locations = locations;
            SelectorId = selectorId;
            SelectorName = selectorName;
            Required = required;
            SelectedLocationId = selectedLocationId;
        }
        
        public LocationSelectorInfo()
        {
        }
    }
}