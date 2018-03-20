using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.PlaceholderServices;
using BackEndServer.Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class CameraController : Controller
    {
        private static AbstractCameraService cameraService = new PlaceholderCameraService();
       
        // GET: /<controller>/
        public IActionResult CameraSelectionForLocation(int locationId)
        {
            CameraInformationList camerasAtLocationModel = cameraService.getCamerasAtLocation(locationId);
            return View(camerasAtLocationModel);
        }

        public IActionResult CameraInformation(int cameraId)
        {
            CameraStatistics cameraStatisticsModel = cameraService.getCameraStatisticsForNowById(cameraId);
            return View(cameraStatisticsModel);
        }

        public IActionResult CameraSelected(int cameraId)
        {
            return RedirectToAction("GraphDashboard", "Graph", new { cameraId });
        }
    }
}
