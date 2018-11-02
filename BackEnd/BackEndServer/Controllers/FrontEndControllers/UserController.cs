using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
        public IActionResult PasswordChange()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            UserPassword userPassword = new UserPassword();
            userPassword.UserId = currentUserId;
            return View("PasswordChange", userPassword);
        }
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }
        public IActionResult PasswordReset([FromQuery(Name ="id")] string token)
        {
            PasswordReset passwordReset = new PasswordReset();
            passwordReset.Token = token;
            return View("PasswordReset", passwordReset);
        }

        [HttpPost]
        public JsonResult InitializePasswordReset(PasswordReset passwordReset)
        {
            return Json(UserService.ResetPassword(passwordReset));
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
        [HttpPost]
        public JsonResult GeneratePasswordResetLink(PasswordResetLink passwordResetLink)
        {
            UserSettings userSettings = UserService.GetUserByEmailAddress(passwordResetLink.Email);
            if(userSettings == null)
            {
                return Json(false);
            }
            else
            {
                return Json(UserService.SendResetPasswordLink(passwordResetLink.Email));
            }
        }
        
        [HttpPost]
        public JsonResult ChangePassword(UserPassword userPassword)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            UserSettings userSettings = UserService.GetUserSettings(currentUserId.Value);
            if(userSettings.Password != userPassword.OldPassword)
            {
                return Json(false);
            }
            if (currentUserId != null)
            {
                userSettings.Password = userPassword.NewPassword;
                return Json(UserService.ModifyPassword(userSettings));
            }
            return Json(false);
        }

        public IActionResult BeginUserCreation()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            return View("UserCreation");
        }
        
        [HttpPost]
        public IActionResult CreateUser(UserSettings userSettings)
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");
            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            UserSettings createdUser = UserService.CreateAndReturnUser(userSettings);
            if (createdUser != null)
            {
                return View("SuccessfulCreation", createdUser);
            }
            return View("Error");
        }
    }
}