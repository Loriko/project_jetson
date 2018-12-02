using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Classes.ErrorResponseClasses;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services;
using BackEndServer.Services.AbstractServices;
using Castle.Core.Internal;

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
        private AlertSummaryService _alertSummaryService;
        private AlertSummaryService AlertSummaryService => _alertSummaryService ?? (_alertSummaryService =
                                                           HttpContext.RequestServices.GetService(typeof(AlertSummaryService)) as
                                                            AlertSummaryService);
                                                            
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
            if (receivedMessage == null)
            {
                return BadRequest(new JsonResult(DataMessageService.CreateInvalidDataMessageResponseBody(receivedMessage)));
            }
            
            // Verify device's API Key.
            if (APIKeyService.VerifyAPIKey(receivedMessage.API_Key) < 0)
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
                DataReceivalResponse receivalResponse = AlertSummaryService.GetReceivalResponse(receivedMessage);
                // Return HTTP OK, without waiting on asynchronous Task.
                return StatusCode(200, new JsonResult(receivalResponse));
            }

            return StatusCode(500, new JsonResult(new FailedPersistResponseBody()));
        }
    }
}