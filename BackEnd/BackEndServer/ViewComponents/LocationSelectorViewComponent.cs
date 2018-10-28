using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Mvc;

namespace BackEndServer.ViewComponents
{
    public class LocationSelectorViewComponent : ViewComponent
    {
        private AbstractLocationService _locationService;
        private AbstractLocationService LocationService => _locationService ?? (_locationService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractLocationService)) as
                                                                 AbstractLocationService);
        
        public IViewComponentResult Invoke(LocationSelectorInfo selectorInfo)
        {
            selectorInfo.Locations = LocationService.GetAvailableLocations();
            return View("LocationSelector", selectorInfo);
        }
    }
}