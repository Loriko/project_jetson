using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.PlaceholderServices;
using System.Collections.Generic;
using System.Configuration;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using Microsoft.AspNetCore.Http;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class LocationController : Controller
    {
        
        private AbstractLocationService _locationService;
        private AbstractLocationService LocationService => _locationService ?? (_locationService =
                                                               HttpContext.RequestServices.GetService(typeof(AbstractLocationService)) as
                                                                   AbstractLocationService);

        // GET
        public IActionResult LocationSelection()
        {
            // TODO: There is a better way to do authentication, investigate and fix this in each controller
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            LocationInformationList locationListModel = LocationService.getAvailableLocationsForUser(currentUsedId.Value);
            return View(locationListModel);
        }

        public IActionResult LocationSelected(int locationId)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            return RedirectToAction("CameraSelectionForLocation", "Camera", new {locationId});
        }
        
        [HttpPost]
        public JsonResult SaveLocation(LocationDetails locationDetails)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId != null)
            {
                return Json(LocationService.SaveLocation(locationDetails));
            }

            return Json(false);
        }

        public IActionResult LoadLocationSelector(string selectorId, string selectorName, bool required, int selectedLocationId)
        {
            return PartialView("LocationSelectorWrapper", new LocationSelectorInfo(selectorId, selectorName, required, selectedLocationId));
        }
    }
}