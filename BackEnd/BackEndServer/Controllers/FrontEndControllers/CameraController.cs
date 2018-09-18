using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.PlaceholderServices;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Common;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class CameraController : Controller
    {
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                               AbstractCameraService);

        // GET: /<controller>/
        public IActionResult CameraSelectionForLocation(int locationId)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            CameraInformationList camerasAtLocationModel = CameraService.getCamerasAtLocation(locationId);
            return View(camerasAtLocationModel);
        }

        public IActionResult CameraInformation(int cameraId)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            CameraStatistics cameraStatisticsModel = CameraService.getCameraStatisticsForNowById(cameraId);
            
            if (cameraStatisticsModel != null)
            {
                return View(cameraStatisticsModel);    
            }
            else
            {
                return View("NoCamera");
            }
        }
        
        public IActionResult CameraSelected(int cameraId)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            
            return RedirectToAction("GraphDashboard", "Graph", new { cameraId });
        }
        
        public IActionResult BeginCameraRegistration()
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            CameraRegistrationDetails registrationDetails =
                CameraService.GetNewCameraRegistrationDetails(HttpContext.Session.GetString("currentUsername"));
            
            return View("CameraRegistration", registrationDetails);
        }

        public IActionResult LoadUserCamera()
        {
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            CameraInformationList availableCameras = CameraService.GetAllCamerasOwnedByUser(currentUsedId.Value);

            return View("ManageCameras", availableCameras);

        }

        [HttpPost]
        public IActionResult RegisterCamera(CameraDetails cameraDetails)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId != null)
            {
                cameraDetails.UserId = currentUsedId.Value;
                CameraService.RegisterCamera(cameraDetails);
//                CameraService.SaveNewCamera(cameraDetails);
            }
            else
            {
                throw new InvalidOperationException("Can't get current user's id!");
            }
            CameraInformationList availableCameras = CameraService.GetAllCamerasOwnedByUser(currentUsedId.Value);
            return View("ManageCameras", availableCameras);
        }
    }
}
