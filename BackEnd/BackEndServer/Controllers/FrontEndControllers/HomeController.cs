using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Models;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services;
using Microsoft.AspNetCore.Http;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class HomeController : Controller
    {
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
            Boolean areCredentialsValid = new AuthenticationService().ValidateCredentials(authenticationModel.Username, authenticationModel.Password);
            if(!areCredentialsValid){
                return View(authenticationModel);    
            }
            else {
                HttpContext.Session.SetString("currentUsername", authenticationModel.Username);
                return RedirectToAction("LocationSelection", "Location");
            }
        }
    }
}