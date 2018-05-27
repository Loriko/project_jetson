using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Models;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services;
using BackEndServer.Services.AbstractServices;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class HomeController : Controller
    {
        private AbstractAuthenticationService _authenticationService;
        private AbstractAuthenticationService AuthenticationService => _authenticationService ?? (_authenticationService =
                                                                           HttpContext.RequestServices.GetService(typeof(AbstractAuthenticationService)) as
                                                                               AbstractAuthenticationService);

        public IActionResult SignIn()
        {
            AuthenticationInformation authenticationModel = new AuthenticationInformation();
            return View(authenticationModel);
        }

        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn");
        }
        
        [HttpPost]
        public IActionResult SignIn(AuthenticationInformation authenticationModel){
            Boolean areCredentialsValid = AuthenticationService.ValidateCredentials(authenticationModel.Username, authenticationModel.Password);
            if(!areCredentialsValid){
                return View(authenticationModel);    
            }
            else {
                HttpContext.Session.SetString("currentUsername", authenticationModel.Username);
                HttpContext.Session.SetInt32("currentUserId", AuthenticationService.GetUserId(authenticationModel.Username).Value);
                return RedirectToAction("LocationSelection", "Location");
            }
        }
    }
}