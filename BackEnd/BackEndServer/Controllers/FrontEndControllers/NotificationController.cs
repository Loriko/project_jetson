using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class NotificationController : Controller
    {
        private AbstractNotificationService _notificationService;
        private AbstractNotificationService NotificationService => _notificationService ?? (_notificationService =
                                                                       HttpContext.RequestServices.GetService(typeof(AbstractNotificationService)) as
                                                                           AbstractNotificationService);
        
        public IActionResult NotificationSelected(int notificationId)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            if (!NotificationService.IsNotificationAcknowledged(notificationId))
            {
                NotificationService.AcknowledgeNotification(notificationId);
            }

            return RedirectToAction("NotificationInformation", "Notification", new {notificationId});
        }

        public IActionResult NotificationInformation(int notificationId)
        {
            if (HttpContext.Session.GetString("currentUsername") == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            NotificationDetails notificationDetails = NotificationService.GetNotificationDetailsById(notificationId);
            
            return View("NotificationInformation", notificationDetails);
//            CameraStatistics cameraStatisticsModel = CameraService.getCameraStatisticsForNowById(cameraId);

//            if (cameraStatisticsModel != null)
//            {
//                return View(cameraStatisticsModel);    
//            }
//            else
//            {
//                return View("NoCamera");
//            }
        }
    }
}