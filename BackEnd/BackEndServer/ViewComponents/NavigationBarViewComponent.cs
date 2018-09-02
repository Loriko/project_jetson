using BackEndServer.Services.AbstractServices;
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
            return View();
        }
    }
}