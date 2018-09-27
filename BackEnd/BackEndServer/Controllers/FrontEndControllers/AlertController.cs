using System;
using System.Collections.Generic;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class AlertController : Controller
    {
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                               AbstractCameraService);
        
        private AbstractAlertService _alertService;
        private AbstractAlertService AlertService => _alertService ?? (_alertService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractAlertService)) as
                                                              AbstractAlertService);
        
        // GET
        public IActionResult Index()
        {
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            CameraInformationList availableCameras = CameraService.getAllCamerasForUser(currentUsedId.Value);
            SortedDictionary<int, List<AlertDetails>> existingAlertsByCameraId = AlertService.GetAllAlertsByCameraIdForUser(currentUsedId.Value);
            AlertDashboardInformation info = new AlertDashboardInformation
            {
                Availablecameras = availableCameras,
                ExistingAlertsByCameraId = existingAlertsByCameraId
            };
            //Check if it's an ajax request for page reloading, meaning we don't want the view + layout, just the view
            //TODO: Create a controller extension class that has a IsAjaxRequest method
            if (Request.Headers["x-requested-with"]=="XMLHttpRequest")
            {
                return PartialView("Dashboard", info);
            }
            
            return View("Dashboard", info);
        }
        
        [HttpPost]
        public IActionResult SaveAlert(AlertDetails alertDetails)
        {
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId != null)
            {
                alertDetails.UserId = currentUsedId.Value;
                AlertService.SaveAlert(alertDetails);
            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }

            return RedirectToAction("Index", "Alert");
        }
        
        public IActionResult DeleteAlert(int alertId)
        {
            AlertService.DeleteAlert(alertId);
            return RedirectToAction("Index", "Alert");
        }

        [HttpPost]
        public JsonResult DisableAlert(AlertDisablingInformation alertDisablingInformation)
        {
            return Json(AlertService.DisableAlert(alertDisablingInformation));
        }
    }
}