using System;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using Castle.Core.Internal;
using System.Threading.Tasks;
using BackEndServer.Models.Enums;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class CameraController : Controller
    {
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                               AbstractCameraService);

        [HttpGet] // /<controller>/
        public IActionResult CameraSelectionForLocation(int locationId)
        {
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            
            CameraInformationList camerasAtLocationModel = CameraService.GetCamerasAtLocationForUser(locationId, currentUsedId.Value);
            if (camerasAtLocationModel.CameraList.IsNullOrEmpty())
            {
                return RedirectToAction("LocationSelection", "Location"); 
            }
            
            return View(camerasAtLocationModel);
        }

        [HttpGet]
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

        [HttpGet]
        public IActionResult CameraSelected(int cameraId)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            PastPeriod pastPeriod = PastPeriod.LastYear;
            return RedirectToAction("GraphDashboard", "Graph", new { cameraId, pastPeriod });
        }
        
        [HttpGet]
        public IActionResult BeginCameraRegistration()
        {
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            CameraRegistrationDetails registrationDetails =
                CameraService.GetNewCameraRegistrationDetails(currentUsedId.Value);
            
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
        public async Task<IActionResult> RegisterCamera(CameraDetails cameraDetails)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            if (currentUserId != null)
            {
                cameraDetails.UserId = currentUserId.Value;
                CameraService.RegisterCamera(cameraDetails);

            }
            else
            {
                return RedirectToAction("SignIn", "Home");
            }
            CameraInformationList availableCameras = CameraService.GetAllCamerasOwnedByUser(currentUserId.Value);
            return View("ManageCameras", availableCameras);
        }

        public IActionResult ModifyCameraRegistration(int cameraId)
        {
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            CameraRegistrationDetails registrationDetails =
                CameraService.GetCameraRegistrationDetailsById(cameraId, currentUsedId.Value);
            
            return View("CameraRegistration", registrationDetails);
        }
    }
}
