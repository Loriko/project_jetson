using System.Collections.Generic;

namespace BackEndServer.Models.ViewModels
{
    public class DataReceivalResponse
    {
        public int NumberOfReceivedStats { get; set; }
        public List<AlertSummary> ActiveAlertsForCamera { get; set; }
    }
}