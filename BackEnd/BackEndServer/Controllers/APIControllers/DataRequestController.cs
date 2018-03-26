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

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller to support requests from the Web Server.
    /// </summary>
    [Route("api/[controller]")] // "api/datarequest"
    public class DataRequestController : Controller
    {
        // Authentication will be considered later in development.

        
        /// <summary>
        /// Returns all statistics between a given Date/Time interval.
        /// Can be called constantly for real-time graph and for on-demand requests.
        /// </summary>
        /// <param name="timeInterval">A TimeInterval object specifying the start and end times in unix time (seconds).</param>
        /// <returns>HTTP Response. May have an empty body, a reponse object or JSON-serialized Data Message containing query results.</returns>
        [HttpPost]
        public IActionResult GetPerSecondStatFromTimeInterval ([FromBody] TimeInterval timeInterval)
        {
            // Validate provided TimeInterval received from Web Server.
            if (timeInterval.isValidTimeInterval() == false)
                return BadRequest( new JsonResult(new InvalidTimeIntervalResponseBody()) );

            // Obtain database context.
            DatabaseQueryService context = HttpContext.RequestServices.GetService(typeof(BackEndServer.Services.DatabaseQueryService)) as DatabaseQueryService;
            
            // Obtain DataMessage using special method of the database context class.
            DataMessage responseMessage = context.getStatsFromInterval(timeInterval);

            if (responseMessage == null)
            {
                return (NoContent());
            }
            else if (responseMessage.getLength() < 1)
            {
                return (NoContent());
            }
            
            // Return HTTP OK along with the JSON-serialized DataMessage object containing all requested query results. 
            return Ok(new JsonResult(responseMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="averagesOfDayRequest"></param>
        /// <returns></returns>
        [HttpPost]
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
        }

        /// <summary>
        /// To request statistics for a single second, call GET and provide a SingleSecondTime object (Json-serialized).
        /// This method does not need its own Request class or Response class, as it can simple take a single SingleSecondTime
        /// object and return a single PerSecondStat object.
        /// </summary>
        /// <param name="singleSecondTime"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPerSecondStatFromSingleSecondTime ([FromBody] SingleSecondTime singleSecondTime)
        {
            if (singleSecondTime.isValidSingleSecondTime() == false)
                return (BadRequest(new JsonResult(new InvalidSingleSecondTimeResponseBody())));

            // Obtain database context.
            DatabaseQueryService context = HttpContext.RequestServices.GetService(typeof(DatabaseQueryService)) as DatabaseQueryService;

            // Obtain AveragesOfDayResponse using special method of the database context class.
            PerSecondStat responseData = context.getSpecificSecond(singleSecondTime);

            if (responseData == null)
                return (NoContent());

            // Return HTTP OK and JSON-serialized Response object.
            return Ok(new JsonResult(responseData));
        }

        [HttpGet("location/{LocationId}", Name = "GetCamerasFromLocation")] // "api/datarequest/location/18"
        public IActionResult GetCamerasFromLocationId (int LocationId)
        {
            // Testing Code:
            // return (Ok(new JsonResult(LocationId)));

            if (LocationId < 0)
            {
                return (BadRequest(new JsonResult(new InvalidLocationIdResponseBody())));
            }

            // Obtain database context.
            DatabaseQueryService context = HttpContext.RequestServices.GetService(typeof(DatabaseQueryService)) as DatabaseQueryService;

            List<DatabaseCamera> camerasForRequestedLocation = context.GetCamerasForLocation(LocationId);

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
