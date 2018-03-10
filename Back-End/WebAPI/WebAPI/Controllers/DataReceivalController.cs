using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Object_Classes;
// using Newtonsoft.Json;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller to receive data storage requests from Capture system.
    /// Use "api/datareceival/datamessage" as the URL.
    /// </summary>
    [Route("api/[controller]/datamessage")]
    public class DataReceivalController : ControllerBase
    {
        /* ALTERNATIVE DOES NOT SATISFY NEEDS
        [HttpPost]
        public string Post()
        {
            string requestBody;
            using (StreamReader streamReader = new StreamReader(Request.Body))
            {
                requestBody = streamReader.ReadToEnd();
            }

            DataMessage dataMessage = JsonConvert.DeserializeObject<DataMessage>(requestBody);
            
            return "Received DataMessage";
        }
        */
        
        /// <summary>
        /// API service that allows a client to store statistics into the database by providing a DataMessage Object.
        /// </summary>
        /// <param name="receivedMessage"></param>
        /// <returns>HTTP Status Code: OK or BAD REQUEST</returns>
        [HttpPost]
        public IActionResult Post([FromBody] DataMessage receivedMessage)
        {
            //return (new JsonResult("Received DataMessage test. Here is the year of persecondstat: " + receivedMessage.RealTimeStats[0].Year));
            
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
