using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEndServer.ViewComponents
{
    public class NavigationBarViewComponent : ViewComponent
    {
        private AbstractNotificationService _notificationService;
        private AbstractNotificationService NotificationService => _notificationService ?? (_notificationService =
                                                                           HttpContext.RequestServices.GetService(typeof(AbstractNotificationService)) as
                                                                           AbstractNotificationService);
        
        public IViewComponentResult Invoke()
        {
            NavigationBarDetails navigationBarDetails = new NavigationBarDetails();
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUsedId != null)
            {
                navigationBarDetails.NotificationList = NotificationService.GetNotificationsForUser(currentUsedId.Value);
            }
            return View("NavigationBar", navigationBarDetails);
        }
    }
}