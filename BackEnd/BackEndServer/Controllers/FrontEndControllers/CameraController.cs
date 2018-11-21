using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Policy;
using Castle.Core.Internal;
using System.Threading.Tasks;
using BackEndServer.Models.Enums;
using BackEndServer.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class CameraController : Controller
    {
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                               AbstractCameraService);
        
        private AbstractUserService _userService;
        private AbstractUserService UserService => _userService ?? (_userService =
                                                       HttpContext.RequestServices.GetService(typeof(AbstractUserService)) as
                                                           AbstractUserService);
        
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

            return RedirectToAction("GraphDashboard", "Graph", new { cameraId });
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

            if (Request.Headers["x-requested-with"]=="XMLHttpRequest")
            {
                return PartialView("ManageCameras", availableCameras);
            }
            
            return View("ManageCameras", availableCameras);
        }

        [HttpPost]
        public JsonResult ValidateCameraKey(string cameraKey)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            if (currentUserId != null)
            {
                return Json(CameraService.ValidateCameraKey(cameraKey));
            }

            return Json(false);
        }

        [HttpPost]
        public IActionResult RegisterCamera(CameraDetails cameraDetails)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            
            cameraDetails.UserId = currentUserId.Value;
            if (!CameraService.RegisterCamera(cameraDetails))
            {
                return View("Error");
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

        [HttpGet]
        public IActionResult ManageCameraKeys()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            if (currentUserId == null || UserService.IsUserAdministrator(currentUserId.Value) == false)
            {
                return RedirectToAction("SignIn", "Home");
            }

            CameraKeyList listOfCameraKeys = CameraService.GetCameraKeyListForAdmin();

            return View("ManageCameraKeys", listOfCameraKeys);
        }

        [HttpGet]
        public IActionResult CreateCameraKey()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            if (currentUserId == null || UserService.IsUserAdministrator(currentUserId.Value) == false)
            {
                return RedirectToAction("SignIn", "Home");
            }

            NewCameraKey newCameraKey = CameraService.GenerateUniqueCameraKey();

            return View("CreateCameraKey", newCameraKey);
        }

        //TODO: should be a post
        [HttpGet]
        public IActionResult DeleteCameraKey(string cameraKey)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            if (currentUserId == null || UserService.IsUserAdministrator(currentUserId.Value) == false)
            {
                return RedirectToAction("SignIn", "Home");
            }

            bool success = CameraService.DeleteCameraFromKey(cameraKey);

            if (success == false)
            {
                return View("Error");
            }

            NewCameraKey deletedCameraKey = new NewCameraKey(cameraKey);

            return View("DeleteCameraKey", deletedCameraKey);
        }
        
        public IActionResult ViewAllUsersExceptCurrent(int cameraId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            List<UserSettings> users = UserService.GetUserSettingsForCamera(cameraId);
            CameraDetails camera = CameraService.GetCameraInfoById(cameraId);
            UserSettingsList settingsList = new UserSettingsList
            {
                CameraDetails = camera,
                UserList = users
            };
            
            if (Request.Headers["x-requested-with"]=="XMLHttpRequest")
            {
                return PartialView("UserViewAccess", settingsList);
            }
            
            return View("UserViewAccess", settingsList);
        }
        
        [HttpPost]
        public IActionResult GiveUserAccess(UserCameraAssociation association)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            return Json(CameraService.TryGiveAccessToUser(association));
        }

        [HttpPost]
        public JsonResult ValidateNewCameraName(int locationId, string cameraName)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId != null)
            {
                return Json(CameraService.ValidateNewCameraName(locationId, cameraName));
            }

            return Json(false);
        }

        [HttpPost]
        public JsonResult UnclaimCamera(int cameraId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId != null)
            {
                return Json(CameraService.UnclaimCamera(cameraId));
            }

            return Json(false);
        }

        [HttpPost]
        public IActionResult RevokeAccess(UserCameraAssociation association)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId != null)
            {
                return Json(CameraService.RevokeAccess(association));
            }

            return Json(false);
        }
    }
}
