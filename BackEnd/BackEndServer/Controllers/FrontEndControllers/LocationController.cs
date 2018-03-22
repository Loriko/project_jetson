using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.PlaceholderServices;
using System.Collections.Generic;
using System.Configuration;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services;
using BackEndServer.Services.HelperServices;
using Microsoft.AspNetCore.Http;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class LocationController : Controller
    {
        // GET
        public IActionResult LocationSelection()
        {
            // TODO: There is a better way to do authentication, investigate and fix this in each controller
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            
            LocationInformationList locationListModel = new LocationService().getAvailableLocationsForUser("johndoe");
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
    }
}