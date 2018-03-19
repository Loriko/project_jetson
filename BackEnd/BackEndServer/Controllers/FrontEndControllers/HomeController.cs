using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Models;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.PlaceholderServices;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class HomeController : Controller
    {
        public IActionResult SignIn()
        {
            AuthenticationInformation authenticationModel = new AuthenticationInformation();
            return View(authenticationModel);
        }

        [HttpPost]
        public IActionResult SignIn(AuthenticationInformation authenticationModel){
            Boolean areCredentialsValid = new PlaceholderAuthenticationService().validateCredentials(authenticationModel.Username, authenticationModel.Password);
            if(!areCredentialsValid){
                return View(authenticationModel);    
            }
            else {
                return RedirectToAction("LocationSelection", "Location");
            }
        }
    }
}