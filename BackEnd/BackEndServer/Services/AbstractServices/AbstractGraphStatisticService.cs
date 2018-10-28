using System;
using System.Collections.Generic;
using BackEndServer.Models.Enums;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractGraphStatisticService
    {
        GraphStatistics GetYearlyGraphStatistics(int cameraId);
        GraphStatistics GetLast30MinutesStatistics(int cameraId);
        GraphStatistics GetGraphStatisticsByInterval(int cameraId, int startDate, int endDate, int interval);
        GraphStatistics GetStatisticsForPastPeriod(int cameraId, PastPeriod pastPeriod, DateTime? startDate = null, DateTime? endDate = null);
        GraphStatistics GetSharedRoomStatisticsForPastPeriod(int roomId, PastPeriod pastPeriod,
            DateTime? startDate = null, DateTime? endDate = null);
        GraphStatistics GetSharedRoomStatisticsForPastPeriod(List<int> cameraIds, PastPeriod pastPeriod,
            DateTime? startDate = null, DateTime? endDate = null);
    }
}
