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
        
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                               HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                                 AbstractCameraService);

        // GET
        public IActionResult LocationSelection()
        {
            // TODO: There is a better way to do authentication, investigate and fix this in each controller
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            LocationInformationList locationListModel = LocationService.GetAvailableLocationsForUser(currentUsedId.Value);
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
                locationDetails.UserId = currentUserId.Value;
                if (LocationService.SaveLocation(locationDetails))
                {
                    return Json(LocationService.GetLocationIdByUserIdAndLocationName(locationDetails.UserId, locationDetails.LocationName));
                }
            }

            return Json(false);
        }

        [HttpPost]
        public JsonResult ValidateNewRoomName(int locationId, string roomName)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId != null)
            {
                return Json(LocationService.ValidateNewRoomName(locationId, roomName));
            }

            return Json(false);
        }

        public IActionResult LoadLocationSelector(string selectorId, string selectorName, bool required, int selectedLocationId)
        {
            return PartialView("LocationSelectorWrapper", new LocationSelectorInfo(selectorId, selectorName, required, selectedLocationId));
        }
        
        public IActionResult LoadRoomSelector(int locationId, string selectorId, string selectorName, bool required, int selectedRoomId)
        {
            return PartialView("RoomSelectorWrapper", new RoomSelectorInfo(locationId, selectorId, selectorName, required, selectedRoomId));
        }

        [HttpPost]
        public JsonResult DeleteLocation(int locationId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId != null)
            {
                return Json(CameraService.DeleteLocationAndUnclaimCameras(locationId));
            }

            return Json(false);
        }
        
        public IActionResult ManageLocations()
        {
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            LocationDetailsList availableLocations = LocationService.GetLocationsCreatedByUser(currentUsedId.Value);

            if (Request.Headers["x-requested-with"]=="XMLHttpRequest")
            {
                return PartialView("ManageLocation", availableLocations);
            }
            
            return View("ManageLocation", availableLocations);
        }

        [HttpPost]
        public JsonResult ValidateNewLocationName(string locationName)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId != null)
            {
                return Json(LocationService.ValidateNewLocationName(locationName, currentUserId.Value));
            }

            return Json(false);
        }
    }
}