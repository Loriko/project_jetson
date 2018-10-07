using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.PlaceholderServices;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class GraphController : Controller
    {
        
        private AbstractGraphStatisticService _graphStatisticService;
        private AbstractGraphStatisticService GraphStatisticService => _graphStatisticService ?? (_graphStatisticService =
                                                                           HttpContext.RequestServices.GetService(typeof(AbstractGraphStatisticService)) as
                                                                               AbstractGraphStatisticService);
        
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                                               AbstractCameraService);

        
        // GET: /<controller>/
        public IActionResult GraphDashboard(int cameraId, PastPeriod pastPeriod)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            CameraInformation cameraInfoWithStatistics = CameraService.GetCameraInformationForPastPeriod(cameraId, pastPeriod);
            return View(cameraInfoWithStatistics);
        }
    }
}
