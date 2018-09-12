using System;
using System.Collections.Generic;
using System.Threading;
using BackEndServer.Classes.EntityDefinitionClasses;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.Enums;
using BackEndServer.Services.AbstractServices;
using Castle.Core.Internal;

namespace BackEndServer.Services
{
    public class AlertMonitoringService
    {
        private readonly IDatabaseQueryService _databaseQueryService;
        private static readonly double ALERT_SNOOZE_HOURS = 2;
        private static readonly int TIME_BETWEEN_ALERT_CHECKS_MS = 30000;
        
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
                
                foreach (var alert in alerts)
                {
                    HandleAlertMonitoring(alert, lastCheckup);
                }
                
                //Check all alerts after last checkup date
                lastCheckup = DateTime.Now;
                Thread.Sleep(TIME_BETWEEN_ALERT_CHECKS_MS);
            }
        }

        private void HandleAlertMonitoring(DatabaseAlert alert, DateTime lastCheckup)
        {
            //check if any persecondstat has a count higher than threshold since last checkup
            DatabasePerSecondStat earliestStatThatTriggersAlert
                = _databaseQueryService.GetEarliestPerSecondStatTriggeringAlert(alert, lastCheckup);

            if (earliestStatThatTriggersAlert != null)
            {
                HandleTriggeringOfAlert(alert, earliestStatThatTriggersAlert);
                //Snooze alert so that it doesn't get retriggered constantly if it monitors a constantly busy area
                SnoozeAlert(alert);
            }
        }

        private void HandleTriggeringOfAlert(DatabaseAlert alert, DatabasePerSecondStat earliestStatThatTriggersAlert)
        {
            if ((ContactMethod) Enum.Parse(typeof(ContactMethod), alert.ContactMethod) == ContactMethod.Notification)
            {
                CreateNotificationForTriggeredAlert(alert, earliestStatThatTriggersAlert);
            }
            else if ((ContactMethod) Enum.Parse(typeof(ContactMethod), alert.ContactMethod) == ContactMethod.Notification)
            {
                //TODO: Send an email
            }
        }

        private void CreateNotificationForTriggeredAlert(DatabaseAlert alert,
            DatabasePerSecondStat earliestStatThatTriggersAlert)
        {
            DatabaseNotification dbNotification = new DatabaseNotification
            {
                AlertId = alert.AlertId,
                Acknowledged = false,
                TriggerDateTime = earliestStatThatTriggersAlert.DateTime
            };
            _databaseQueryService.PersistNewNotification(dbNotification);
        }

        private void SnoozeAlert(DatabaseAlert alert)
        {
            alert.SnoozedUntil = DateTime.Now.AddHours(ALERT_SNOOZE_HOURS);
            _databaseQueryService.PersistExistingAlert(alert);
        }
    }
}