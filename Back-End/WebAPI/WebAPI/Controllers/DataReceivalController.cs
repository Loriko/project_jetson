using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebAPI.Object_Classes;
using WebAPI.Models;
using WebAPI.Error_Response_Classes;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller to receive data storage requests from a Capture system (Jetson TX2 or other).
    /// </summary>
    [Route("api/[controller]")] // "api/datareceival"
    public class DataReceivalController : ControllerBase
    {
        /// <summary>
        /// API service that allows a capture system to store statistics into the database by providing a DataMessage Object.
        /// </summary>
        /// <param name="receivedMessage">DataMessage object received from a capture system. May contain PerSecondStats from multiple cameras.</param>
        /// <returns>HTTP Status Code: OK or BAD REQUEST</returns>
        [HttpPost]
        [Route("datamessage")]
        public IActionResult Persist([FromBody] DataMessage receivedMessage)
        {
            // DO NOT DELETE
            // For testing purposes, use this line of code:
            // return (new JsonResult("Received DataMessage test. Here is the year of persecondstat: " + receivedMessage.RealTimeStats[0].Year));
            
            if (receivedMessage.isValidDataMessage() == false)
            {
                string[] invalidAttributes = receivedMessage.getInvalidAttributes();
                InvalidDataMessageResponseBody invalidResponseBody = new InvalidDataMessageResponseBody(invalidAttributes);
                return (BadRequest(new JsonResult(invalidResponseBody)));
            }

            // Obtain database context.
            StatisticsDatabaseContext context = HttpContext.RequestServices.GetService(typeof(WebAPI.Models.StatisticsDatabaseContext)) as StatisticsDatabaseContext;

            // Store the PerSecondStats objects of the DataMessage to the database and receive a boolean indicating if operation was successful (true) or not (false).
            bool wasPersistSuccesful = context.storeStatsFromMessage(receivedMessage);

            if (wasPersistSuccesful)
            {
                return (Ok());
            }

            // Else, database persist problem.
            FailedPersistResponseBody persistBody = new FailedPersistResponseBody();

            return (StatusCode(500, new JsonResult(persistBody)));
        }
    }
}
