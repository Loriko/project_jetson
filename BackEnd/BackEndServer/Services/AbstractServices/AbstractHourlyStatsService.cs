using System;
using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Services.AbstractServices
{
    interface AbstractHourlyStatsService
    {
        void AutoCalculateHourlyStats(DataMessage dataMessage);
        List<DateTime> GetHoursToBeCalculated(List<PerSecondStat> uniqueCameraStats);
        List<DatabasePerHourStat> CalculateHourlyAverages(List<DateTime> hoursToCalulate, int cameraId); 
    }
}