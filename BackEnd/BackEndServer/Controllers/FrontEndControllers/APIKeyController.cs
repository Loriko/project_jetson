using Microsoft.AspNetCore.Mvc;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BackEndServer.Controllers.FrontEndControllers
{
    public class APIKeyController : Controller
    {
        private AbstractAPIKeyService _apiKeyService;
        private AbstractAPIKeyService APIKeyService => _apiKeyService ?? (_apiKeyService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractAPIKeyService)) as
                                                               AbstractAPIKeyService);
        [HttpGet]
        public IActionResult ManageAPIKeys()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            // TODO: Ensure User is an ADMIN

            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            return View("ManageAPIKeys");
        }

        [HttpGet]
        public IActionResult CreateAPIKey()
        {
            int? currentUserId = HttpContext.Session.GetInt32("currentUserId");

            // TODO: Ensure User is an ADMIN

            if (currentUserId == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            // Randomly generate API Key registered to ADMIN, save it to database in salted and hashed format, return plain text key.
            string plainTextAPIKey = APIKeyService.RegisterNewAPIKey(currentUserId);

            NewAPIKey newAPIKey = new NewAPIKey(plainTextAPIKey);

            return View("CreateAPIKey", newAPIKey);
        }
    }
}