using System;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
using BackEndServer.Models.ViewModels;
using BackEndServer.Models.DBModels;
using Microsoft.AspNetCore.Http;
using System.IO;

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
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            CameraInformationList camerasAtLocationModel = CameraService.getCamerasAtLocation(locationId);
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
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            CameraRegistrationDetails registrationDetails =
                CameraService.GetNewCameraRegistrationDetails(HttpContext.Session.GetString("currentUsername"));
            
            return View("CameraRegistration", registrationDetails);
        }

        [HttpPost]
        public IActionResult RegisterCamera(CameraDetails cameraDetails)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            if (currentUserId != null)
            {
                cameraDetails.UserId = currentUserId.Value;

                #region File Upload Verification

                IFormFile image = cameraDetails.UploadedImage;

                // If the user has uploaded a file.
                if (image != null)
                {
                    // Verify file size, must be under 5 MB.
                    if (image.Length > 5000000)
                    {
                        // Return error page or process form without the uploaded file?
                    }

                    // Verify that the file is a valid image file (respects Minimum Size, File Extension and MIME Types).
                    if (HttpPostedFileBaseExtensions.IsImage(image))
                    {
                        // Proceed to process the request with the valid image.

                        // Obtain the file extension.
                        string fileExtension = Path.GetExtension(image.FileName).ToLower();

                        // Obtain the Database ID of the camera.
                        int cameraId = CameraService.GetExistingCameraId(cameraDetails.CameraKey);

                        // Save the file to disk.

                        // 1. Ensure the output folder exists.
                        DirectoryInfo outputDirectory = Directory.CreateDirectory(DatabaseCamera.PATH_FOR_USER_UPLOADED_IMAGES);

                        // 2. Create the full file path (output path + filename).
                        string fullFilePath = Path.Combine(outputDirectory.FullName, (cameraId + fileExtension));
                        cameraDetails.SavedImagePath = fullFilePath;

                        // 3. Save IFormFile as an image file in the output path.
                        using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                        {
                            // NOTE: If this was for the Edit page, we would have to delete the previous picture first.
                            image.CopyToAsync(fileStream);
                        }
                    }
                }
            
                #endregion

                CameraService.RegisterCamera(cameraDetails);

                //                CameraService.SaveNewCamera(cameraDetails);
            }
            else
            {
                throw new InvalidOperationException("Can't get current user's id!");
            }

            return View("CameraRegistration", CameraService.GetNewCameraRegistrationDetails(HttpContext.Session.GetString("currentUsername")));
        }
    }
}
