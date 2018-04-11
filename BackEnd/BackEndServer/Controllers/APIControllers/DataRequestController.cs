using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackEndServer.Classes.DataRequestClasses;
using BackEndServer.Classes.DataResponseClasses;
using BackEndServer.Classes.ErrorResponseClasses;
using BackEndServer.Services.HelperServices;
using BackEndServer.Models.DBModels;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Services;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller to support requests from the Web Server.
    /// </summary>
    [Route("api/[controller]")] // "api/datarequest"
    public class DataRequestController : Controller
    {
        private AbstractCameraService _cameraService;
        private AbstractCameraService CameraService => _cameraService ?? (_cameraService =
                                                           HttpContext.RequestServices.GetService(typeof(AbstractCameraService)) as
                                                               AbstractCameraService);

        private AbstractDataMessageService _dataMessageService;
        private AbstractDataMessageService DataMessageService => _dataMessageService ?? (_dataMessageService =
                                                                     HttpContext.RequestServices.GetService(typeof(AbstractDataMessageService)) as
                                                                         AbstractDataMessageService);
        
        [HttpPost]
        public IActionResult GetPerSecondStatsFromTimeInterval ([FromBody] TimeInterval unverifiedTimeInterval)
        {
            if (DataMessageService.checkTimeIntervalValidity(unverifiedTimeInterval) == false)
            {
                return BadRequest(new JsonResult(new InvalidTimeIntervalResponseBody()));
            }

            DataMessage responseBody = DataMessageService.retrievePerSecondStatsBetweenInterval(unverifiedTimeInterval);

            if (responseBody.IsEmpty())
            {
                return (NoContent());
            }
            
            return Ok(new JsonResult(responseBody));
        }

        [HttpGet("mostrecentstat/{cameraId}", Name = "GetMostRecentPerSecondStatForCamera")] // "api/datarequest/mostrecentstat/4"
        public IActionResult GetMostRecentPerSecondStatForCameraId(int cameraId)
        {
            CameraStatistics stat = CameraService.getCameraStatisticsForNowById(cameraId);

            if (stat == null)
            {
                return NoContent();
            }

            return Ok(new JsonResult(stat));
        }

        [HttpGet("location/{locationId}", Name = "GetCamerasFromLocation")] // "api/datarequest/location/18"
        public IActionResult GetCamerasFromLocationId(int locationId)
        {
            // Testing Code:
            // return (Ok(new JsonResult(locationId)));

            if (locationId < 0)
            {
                return (BadRequest(new JsonResult(new InvalidLocationIdResponseBody())));
            }

            List<DatabaseCamera> camerasForRequestedLocation = CameraService.getDatabaseCamerasAtLocation(locationId);

            // TODO: use list of EntityDefinitionClasses.Camera instead

            if (camerasForRequestedLocation == null)
            {
                return (NoContent());
            }
            else if (camerasForRequestedLocation.Count == 0)
            {
                return (NoContent());
            }

            return (Ok(new JsonResult(camerasForRequestedLocation)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="averagesOfDayRequest"></param>
        /// <returns></returns>
        /*[HttpPost]
        public IActionResult GetHourlyAveragesForDayFromRequest ([FromBody] AveragesOfDayRequest averagesOfDayRequest)
        {
            // Validate the date which the hourly averages are being requested from Web Server.
            if (averagesOfDayRequest.isValidRequest() == false)
                return BadRequest(new JsonResult( new InvalidAveragesOfDayRequestResponseBody()));

            // Obtain database context.
            DatabaseQueryService context = HttpContext.RequestServices.GetService(typeof(DatabaseQueryService)) as DatabaseQueryService;

            // Obtain AveragesOfDayResponse using special method of the database context class.
            AveragesOfDayResponse responseData = context.getHourlyAveragesForDay(averagesOfDayRequest);

            if (responseData == null)
            {
                return (NoContent());
            }
            else if (responseData.HourlyAverages.Length < 1)
            {
                return (NoContent());
            }

            // Return HTTP OK and JSON-serialized Response object.
            return Ok(new JsonResult(responseData));
        }*/

        /*
        /// <summary>
        /// For testing connection to the database...
        /// PLEASE IGNORE
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            // Ensure the Default Connection String matches your SQL Local Server. Look at appsettings.json

            StatisticsDatabaseContext context = HttpContext.RequestServices.GetService(typeof(WebAPI.Models.StatisticsDatabaseContext)) as StatisticsDatabaseContext;

            return ("test");
        }
        */
    }
}
