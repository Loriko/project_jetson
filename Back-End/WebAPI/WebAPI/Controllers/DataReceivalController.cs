using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Object_Classes;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller to receive data storage requests from a Capture system (Jetson TX2 or other).
    /// Use "api/datareceival/datamessage" as the URL path.
    /// </summary>
    [Route("api/[controller]/datamessage")]
    public class DataReceivalController : ControllerBase
    {
        /// <summary>
        /// API service that allows a capture system to store statistics into the database by providing a DataMessage Object.
        /// </summary>
        /// <param name="receivedMessage">DataMessage object received from a capture system. May contain PerSecondStats from multiple cameras.</param>
        /// <returns>HTTP Status Code: OK or BAD REQUEST</returns>
        [HttpPost]
        public IActionResult Post([FromBody] DataMessage receivedMessage)
        {
            // For testing purposes, use this line of code:
            //return (new JsonResult("Received DataMessage test. Here is the year of persecondstat: " + receivedMessage.RealTimeStats[0].Year));
            
            if (receivedMessage.isValidMessage() == false)
                return (BadRequest(new JsonResult("There is a problem with the data you provided. Please verify your DataMessage object and its contents.")));

            // Obtain database context.
            StatisticsDatabaseContext context = HttpContext.RequestServices.GetService(typeof(WebAPI.Models.StatisticsDatabaseContext)) as StatisticsDatabaseContext;

            // Store the PerSecondStats objects of the DataMessage to the database and receive a boolean indicating if operation was successful (true) or not (false).
            bool wasPersistSuccesful = context.storeStatsFromMessage(receivedMessage);

            if (wasPersistSuccesful)
            {
                return ( Ok( new JsonResult("Data Message's contents stored succesfully to database.") ) );
            }

            // Else, bad client request (problem with DataMessage object or something else).
            return (BadRequest(new JsonResult("There was a problem storing the data you provided into the database. Please verify your DataMessage object and its contents.")));
        }
    }
}
