using BackEndServer.Models.ViewModels;
using BackEndServer.Services;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class UserController : Controller
    {
        private AbstractUserService _userService;
        private AbstractUserService UserService => _userService ?? (_userService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractUserService)) as
                                                           AbstractUserService);
        
        public IActionResult BeginUserSettingsModification()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            UserSettings userSettings = UserService.GetUserSettings(currentUserId.Value);
            return View("UserSettings", userSettings);
        }

        [HttpPost]
        public JsonResult ModifyUserSettings(UserSettings userSettings)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId != null)
            {
                return Json(UserService.ModifyUser(userSettings));
            }
            
            return Json(false);
        }
    }
}