using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Object_Classes;
using WebAPI.Models;

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
            StatisticsDatabaseContext context = HttpContext.RequestServices.GetService(typeof(WebAPI.Models.StatisticsDatabaseContext)) as StatisticsDatabaseContext;

            // Database Query to get all per second stats objects.
            List<TestObject> perSecondStatsList = context.testDatabase();

            return (new JsonResult(perSecondStatsList));
        }
    }
}