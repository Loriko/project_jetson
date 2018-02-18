using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Object_Classes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public DataMessage Get ([FromBody] TimeInterval timeInterval)
        {
            // Validate correct interval, return error if not valid.

            // Query database for all results between interval and group into a DataMessage object.

            // Return data message object.

            // Not sure if every single second is required by front end...

            return (null);
        }

    }
}
