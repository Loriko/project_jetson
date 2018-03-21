using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Models.DBModels;
using BackEndServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;

namespace WebAPI.Controllers
{
    // Controller for testing the database connection. Use: api/databasetest
    [Route("api/[controller]")]
    public class DatabaseTestController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            // Ensure the Default Connection String matches your SQL Local Server. Look at appsettings.json
            DatabaseQueryService context = HttpContext.RequestServices.GetService(typeof(DatabaseQueryService)) as DatabaseQueryService;

            // Database Query to get all per second stats objects.
            List<TestObject> perSecondStatsList = context.testDatabase();

            return (new JsonResult(perSecondStatsList));
        }
    }
}