using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractGraphStatisticService
    {
        GraphStatistics GetYearlyGraphStatistics(int cameraId);
        GraphStatistics GetLast30MinutesStatistics(int cameraId);
    }
}
