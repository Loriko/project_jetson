using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Models.ViewModels;
using System.Collections;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractGraphStatisticService
    {
        GraphStatistics GetYearlyGraphStatistics(int cameraId);
        GraphStatistics GetLast30MinutesStatistics(int cameraId);
    }
}
