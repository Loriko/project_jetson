using System;
namespace BackEndServer.Models.ViewModels
{
    public class CameraStatistics
    {
        // We should probably change this to a MySQL string DateTime.
        public DateTime? LastUpdatedTime { get; set; }
        public int MostRecentPeopleCount { get; set; }
        public bool DayTimeOfTheWeekAverageCountAvailable { get; set; }
        public int DayTimeOfTheWeekAverageCount { get; set; }
        public string DayTimeOfTheWeekAverageCountDisplayString { get; set; }
        public bool PeriodOfTheDayAverageCountAvailable { get; set; }
        public int PeriodOfTheDayAverageCount { get; set; }
        public string PeriodOfTheDayAverageCountDisplayString { get; set; }
        public CameraInformation CameraInformation { get; set; }
    }
}
