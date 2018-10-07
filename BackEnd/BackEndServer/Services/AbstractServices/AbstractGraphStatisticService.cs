using BackEndServer.Models.Enums;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractGraphStatisticService
    {
        GraphStatistics GetYearlyGraphStatistics(int cameraId);
        GraphStatistics GetLast30MinutesStatistics(int cameraId);
        GraphStatistics GetGraphStatisticsByInterval(int cameraId, int startDate, int endDate, int interval);
        GraphStatistics GetStatisticsForPastPeriod(int cameraId, PastPeriod pastPeriod);
    }
}
