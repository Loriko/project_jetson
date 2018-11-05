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
        
        private AbstractUserService _userService;
        private AbstractUserService UserService => _userService ?? (_userService =
                                                       HttpContext.RequestServices.GetService(typeof(AbstractUserService)) as
                                                           AbstractUserService);
        
        public IViewComponentResult Invoke()
        {
            int? currentUsedId = HttpContext.Session.GetInt32("currentUserId");
            NavigationBarDetails navigationBarDetails = UserService.GetNavigationBarDetailsForUser(currentUsedId);
            return View("NavigationBar", navigationBarDetails);
        }
    }
}