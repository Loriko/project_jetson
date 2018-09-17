using System;
namespace BackEndServer.Models.ViewModels
{
    public class CameraStatistics
    {
        public DateTime? LastUpdatedTime { get; set; }
        public int? MostRecentPeopleCount { get; set; }
        public bool DayTimeOfTheWeekAverageCountAvailable { get; set; }
        public int DayTimeOfTheWeekAverageCount { get; set; }
        public string DayTimeOfTheWeekAverageCountDisplayString { get; set; }
        public bool PeriodOfTheDayAverageCountAvailable { get; set; }
        public int PeriodOfTheDayAverageCount { get; set; }
        public string PeriodOfTheDayAverageCountDisplayString { get; set; }
        public CameraInformation CameraInformation { get; set; }
        public CameraDetails CameraDetails { get; set; }
        public GraphStatistics GraphStatistics { get; set; }
    }
}
