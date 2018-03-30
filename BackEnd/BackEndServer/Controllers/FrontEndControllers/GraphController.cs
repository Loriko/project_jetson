using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.PlaceholderServices;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class GraphController : Controller
    {
        // GET: /<controller>/
        public IActionResult GraphDashboard(int cameraId)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            
            GraphStatistics graphStatistics = new PlaceholderGraphStatisticsService().getMaxStatistics(cameraId);
            return View(graphStatistics);
        }
    }
}
