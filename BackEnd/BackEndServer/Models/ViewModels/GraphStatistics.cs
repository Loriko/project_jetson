using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Models.Enums;

namespace BackEndServer.Models.ViewModels
{
    public class GraphStatistics
    {
        public string[][] Stats { get; set; }
        public PastPeriod SelectedPeriod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
