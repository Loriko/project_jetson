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
        public IActionResult GraphDashboard(int cameraId, PastPeriod pastPeriod = PastPeriod.LastYear, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            CameraInformation cameraInfo = CameraService.getCameraInformationById(cameraId);
            cameraInfo.GraphStatistics.SelectedPeriod = pastPeriod;
            cameraInfo.GraphStatistics.StartDate = startDate;
            cameraInfo.GraphStatistics.EndDate = endDate;
            return View(cameraInfo);
        }

        public JsonResult GetCustomGraphStats(int cameraId, DateTime startDate, DateTime endDate)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return Json(false);
            }

            GraphStatistics statistics =
                GraphStatisticService.GetStatisticsForPastPeriod(cameraId, PastPeriod.Custom, startDate, endDate);
            return Json(statistics);
        }
        
        public JsonResult GetGraphStats(int cameraId, PastPeriod pastPeriod)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return Json(false);
            }
            GraphStatistics statistics =
                GraphStatisticService.GetStatisticsForPastPeriod(cameraId, pastPeriod);
            return Json(statistics);
        }

        public JsonResult GetSharedRoomCustomGraphStats(int roomId, DateTime startDate, DateTime endDate)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return Json(false);
            }

            GraphStatistics statistics =
                GraphStatisticService.GetSharedRoomStatisticsForPastPeriod(roomId, PastPeriod.Custom, startDate, endDate);
            return Json(statistics);
        }
        
        public JsonResult GetSharedRoomGraphStats(int roomId, PastPeriod pastPeriod)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return Json(false);
            }
            GraphStatistics statistics =
                GraphStatisticService.GetSharedRoomStatisticsForPastPeriod(roomId, pastPeriod);
            return Json(statistics);
        }
        
        public IActionResult SharedRoomGraphDashboard(int roomId, PastPeriod pastPeriod = PastPeriod.LastYear, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            SharedGraphStatistics sharedGraphStatistics = CameraService.GetSharedRoomGraphStatistics(roomId);
            sharedGraphStatistics.SelectedPeriod = pastPeriod;
            sharedGraphStatistics.StartDate = startDate;
            sharedGraphStatistics.EndDate = endDate;
            return View("RoomStatistics", sharedGraphStatistics);
        }
    }
}
