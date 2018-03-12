using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Object_Classes
{
    public enum StatusCodes
    {
        Success = 1,
        UnableToPersistReceivedData = 20,
        InvalidDataProvided = 21,
        LoginFailure = 22
    }

    public enum RequestTypes
    {
        DataPersistRequest,
        PerSecondStatsBetweenIntervalRequest,
        HourlyAveragesForDayRequest,
        DailyAveragesForWeekRequest
    }

    public class BasicResponse
    {
        public int StatusCode { get; set; }
        public string RequestType { get; set; }
        public string Message { get; set; }

        public BasicResponse(int StatusCode, string RequestType, string Message="")
        {
            this.StatusCode = StatusCode;
            this.RequestType = RequestType;
            this.Message = Message;
        }
    }
}
