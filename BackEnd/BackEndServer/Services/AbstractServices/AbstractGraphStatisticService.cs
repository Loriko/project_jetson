using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEndServer.Models.ViewModels;
using System.Collections;

namespace BackEndServer.Services.AbstractServices
{
    interface AbstractGraphStatisticService
    {
        GraphStatistics getMaxStatistics(int cameraId);
    }
}
