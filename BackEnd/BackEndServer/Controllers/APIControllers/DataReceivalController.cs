using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Classes.ErrorResponseClasses;
using BackEndServer.Services.AbstractServices;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller to receive data storage requests from a Capture system (Jetson TX2 or other).
    /// </summary>
    [Route("api/[controller]")] // "api/datareceival"
    public class DataReceivalController : ControllerBase
    {
        #region Services
        private AbstractDataMessageService _dataMessageService;
        private AbstractDataMessageService DataMessageService => _dataMessageService ?? (_dataMessageService =
                                                               HttpContext.RequestServices.GetService(typeof(AbstractDataMessageService)) as
                                                                         AbstractDataMessageService);
        private AbstractAPIKeyService _apiKeyService;
        private AbstractAPIKeyService APIKeyService => _apiKeyService ?? (_apiKeyService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractAPIKeyService)) as
                                                               AbstractAPIKeyService);
        #endregion

        /// <summary>
        /// API service that allows a capture system to store statistics into the database by providing a DataMessage Object.
        /// </summary>
        /// <param name="receivedMessage">DataMessage object received from a capture system. May contain PerSecondStat from multiple cameras.</param>
        /// <returns>HTTP Response (401, 400, 200 or 500) with or without response object in body.</returns>
        [HttpPost]
        [Route("DataMessage")] // "api/datareceival/datamessage"
        public IActionResult DataMessage([FromBody] DataMessage receivedMessage)
        {
            // Verify device's API Key.
            if (_apiKeyService.VerifyAPIKey(receivedMessage.API_Key) < 0)
            {
                // If API Key does not exist or is deactivated.
                return Unauthorized();
            }

            if (DataMessageService.CheckDataMessageValidity(receivedMessage) == false)
            {
                return BadRequest(new JsonResult(DataMessageService.CreateInvalidDataMessageResponseBody(receivedMessage)));
            }
            else if (DataMessageService.StoreStatsFromDataMessage(receivedMessage))
            {
                // Asynchronously check hourly stats are ready for calculation and perform if needed.
                //Task hourlyStatsCheck = Task.Factory.StartNew(
                    //() => _hourlyStatsService.AutoCalculateHourlyStats(receivedMessage));
                
                // Return HTTP OK, without waiting on asynchronous Task.
                return Ok("Received datamessage with " + receivedMessage.GetLength() + " per second stats.");
            }

            return StatusCode(500, new JsonResult(new FailedPersistResponseBody()));
        }
    }
}