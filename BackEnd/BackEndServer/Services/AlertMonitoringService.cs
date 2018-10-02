using System;
using System.Collections.Generic;
using System.Threading;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.Enums;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using BackEndServer.Services.HelperServices;
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
            //TODO: Will need to get the value from the database
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
            else if ((ContactMethod) Enum.Parse(typeof(ContactMethod), alert.ContactMethod) == ContactMethod.Email)
            {
                if (!SendAlertTriggeredEmail(alert, earliestStatThatTriggersAlert))
                {
                    CreateNotificationForTriggeredAlert(alert, earliestStatThatTriggersAlert, true);
                }
            }
        }

        private bool SendAlertTriggeredEmail(DatabaseAlert alert, DatabasePerSecondStat earliestStatThatTriggersAlert)
        {
            DatabaseUser databaseUser = _databaseQueryService.GetUserById(alert.UserId);
            if (!databaseUser.EmailAddress.IsNullOrEmpty())
            {
                string emailSubject = GetEmailSubject(alert, earliestStatThatTriggersAlert);
                string emailBody = GetEmailBody(alert, earliestStatThatTriggersAlert, databaseUser);
                return EmailService.SendEmail(databaseUser.EmailAddress, emailSubject, emailBody);
            }
            
            return false;
        }

        private string GetEmailBody(DatabaseAlert alert, DatabasePerSecondStat earliestStatThatTriggersAlert, DatabaseUser user)
        {
            AlertTriggeredEmailInformation alertEmailInfo =
                GetAlertTriggeredInformation(alert, earliestStatThatTriggersAlert, user);
            string emailBody =
                RazorEngineWrapper.RunCompile("Views/Alert", "AlertTriggeredEmailBodyTemplate.cshtml", alertEmailInfo);
            return emailBody;
        }

        private static string GetEmailSubject(DatabaseAlert alert, DatabasePerSecondStat earliestStatThatTriggersAlert)
        {
            return $"Your alert '{alert.AlertName}' was triggered on {earliestStatThatTriggersAlert.DateTime.ToShortTimeString()}";
        }

        private AlertTriggeredEmailInformation GetAlertTriggeredInformation(DatabaseAlert alert, 
            DatabasePerSecondStat earliestStatThatTriggersAlert, DatabaseUser user)
        {
            DatabaseCamera camera = _databaseQueryService.GetCameraById(alert.CameraId);
            AlertTriggeredEmailInformation alertEmailInformation = new AlertTriggeredEmailInformation
            {
                CameraName = camera.CameraName,
                AlertName = alert.AlertName,
                DateTriggered = earliestStatThatTriggersAlert.DateTime
            };
            
            alertEmailInformation.Name = GetUserName(user);

            if (camera.LocationId != null)
            {
                alertEmailInformation.LocationName = _databaseQueryService.GetLocationById(camera.LocationId.Value).LocationName;
            }

            return alertEmailInformation;
        }

        private static string GetUserName(DatabaseUser user)
        {
            string name;
            if (!user.FirstName.IsNullOrEmpty())
            {
                name = user.FirstName;
                if (!user.LastName.IsNullOrEmpty())
                {
                    name += " " + user.LastName;
                }   
            }
            else if (!user.LastName.IsNullOrEmpty())
            {
                name = user.LastName;
            }
            else
            {
                name = user.Username;
            }
            return name;
        }

        private void CreateNotificationForTriggeredAlert(DatabaseAlert alert,
            DatabasePerSecondStat earliestStatThatTriggersAlert, bool failedEmail = false)
        {
            DatabaseNotification dbNotification = new DatabaseNotification
            {
                AlertId = alert.AlertId,
                Acknowledged = false,
                TriggerDateTime = earliestStatThatTriggersAlert.DateTime,
                FailedEmail = failedEmail
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