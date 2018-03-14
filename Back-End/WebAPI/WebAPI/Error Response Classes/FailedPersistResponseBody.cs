using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Error_Response_Classes
{
    public class FailedPersistResponseBody
    {
        string Message;

        public FailedPersistResponseBody()
        {
            this.Message = "Unable to persist received data to the database.";
        }
    }
}
