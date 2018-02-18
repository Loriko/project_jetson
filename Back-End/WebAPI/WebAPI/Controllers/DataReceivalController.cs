using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Object_Classes;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller to receive data storage requests from Capture system.
    /// </summary>
    [Route("api/[controller]")]
    public class DataReceivalController : ControllerBase
    {
        /* POST TESTER method to see if controller is working. */
        // POST a string to: api/datareceival
        [HttpPost]
        public string Post([FromBody]string value)
        {
            return ("Data Receival controller accessed !!! You have submitted the following string via POST request: " + value);
        }

        /* GET TESTER method to see if controller is accessible via browser. */
        // GET request to: api/datareceival
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "nvidia", "is better", "than", "rasberry pi" };
        }

        /* Hello World version of the data receiving process. Client must sent a JSON serialized DataMessage object. */
        [HttpPost]
        public string Post([FromBody]DataMessage receivedMessage)
        {
            string firstSecondStats = receivedMessage.RealTimeStats.ToString();
            return ("The first second in the data message contains: " + firstSecondStats);
        }
    }
}
