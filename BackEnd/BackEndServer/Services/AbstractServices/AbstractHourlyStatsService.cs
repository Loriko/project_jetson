using System;
using System.Collections.Generic;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Services.AbstractServices
{
    interface AbstractHourlyStatsService
    {
        void AutoCalculateHourlyStats(DataMessage dataMessage);
        List<DateTime> CheckIfCalculationsRequired(DataMessage dataMessage);
        List<DatabasePerHourStat> CalculateHourlyAverages(List<DateTime> hoursToCalulate); 
    }
}