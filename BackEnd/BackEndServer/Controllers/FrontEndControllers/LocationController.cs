using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.PlaceholderServices;
using System.Collections.Generic;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class LocationController : Controller
    {
        // GET
        public IActionResult LocationSelection()
        {
            LocationInformationList locationListModel = new PlaceholderLocationService().getAvailableLocationsForUser("johndoe");
            return View(locationListModel);
        }

        public IActionResult LocationSelected(int locationId)
        {
            return RedirectToAction("CameraSelectionForLocation", "Camera", new {locationId});
        }
    }
}