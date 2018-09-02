using System;
using System.Collections.Generic;
using System.Threading;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;
using BackEndServer.Services.AbstractServices;
using Castle.Core.Internal;

namespace BackEndServer.Services
{
    public class AlertMonitoringService
    {
        private readonly IDatabaseQueryService _databaseQueryService;

        public AlertMonitoringService(IDatabaseQueryService databaseQueryService)
        {
            _databaseQueryService = databaseQueryService;
        }
        
        public void StartMonitoring()
        {
            //Will need to get the value from the database
            DateTime lastCheckup = DateTime.MinValue;
            while (true)
            {
                
                //First get all alerts
                List<DatabaseAlert> alerts = _databaseQueryService.GetAllAlerts();
                Console.WriteLine("Monitoring task retrieved the following alerts for checkup:" + alerts);
                
                foreach (var alert in alerts)
                {
                    //check if any persecondstat has a count higher than threshold since last checkup
                    List<DatabasePerSecondStat> perSecondStats 
                        = _databaseQueryService.GetPerSecondStatsTriggeringAlert(alert, lastCheckup);

                    if (!perSecondStats.IsNullOrEmpty())
                    {
                        //Probably not needed, just needed to know if it's not empty
                        //Will lately on handle alert's by using it's triggering method
                        foreach (var stat in perSecondStats)
                        {
                            Console.WriteLine($"\nStat triggering alert: {stat}\n");
                        }
                    }
                }
                
                //Check all alerts after last checkup date
                lastCheckup = DateTime.Now;
                Thread.Sleep(30000);
            }
        }
        
        
    }
}