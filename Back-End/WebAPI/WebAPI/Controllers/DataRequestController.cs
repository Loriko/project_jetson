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
    /// <summary>
    /// Controller to support requests from the Front-End.
    /// </summary>
    [Route("api/[controller]")]
    public class DataRequestController : Controller
    {
        // Authentication will be considered later in development.

        /// <summary>
        /// Returns all statistics between a given Date/Time interval. Uses 24 hour clock.
        /// </summary>
        // GET statistics between a specified interval. (called constantly for real-time graph and for on-demand requests.)
        [HttpGet]
        public IActionResult Get ([FromBody] TimeInterval timeInterval)
        {
            if (timeInterval.isValidInterval() == false)
                return BadRequest();


            // Query database for all results between interval and group into a DataMessage object.

            // obtain count of number of results

            // Return data message object.

            // Not sure if every single second is required by front end...

            // Create DataMessage and fill in attributes.



            // Convert to JSON response

            #region Test code
            PerSecondStats testSec = new PerSecondStats(1, 2018, 2, 18, 10, 48, 55,6);

            DataMessage test = new DataMessage(1);

            test.RealTimeStats[0] = testSec;

            JsonResult jsonStats = new JsonResult(test);

            return (jsonStats);

            #endregion
        }

    }
}
