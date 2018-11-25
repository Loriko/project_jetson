using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
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
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return Content(string.Empty);
            }
            selectorInfo.Locations = LocationService.GetLocationCreatedByUserInformationList(currentUsedId.Value);
            return View("LocationSelector", selectorInfo);
        }
    }
}