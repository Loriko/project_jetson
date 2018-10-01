using System;
using BackEndServer.Models.DBModels;

namespace BackEndServer.Models.ViewModels
{
    public class NotificationDetails
    {
        public int NotificationId { get; set; }
        public int AlertId { get; set; }
        public AlertDetails Alert { get; set; }
        public DateTime TriggerDateTime { get; set; }
        public bool Acknowledged { get; set; }
        public bool FailedEmail { get; set; }

        public NotificationDetails()
        {
        }

        public NotificationDetails(DatabaseNotification dbNotification)
        {
            NotificationId = dbNotification.NotificationId;
            AlertId = dbNotification.AlertId;
            TriggerDateTime = dbNotification.TriggerDateTime;
            Acknowledged = dbNotification.Acknowledged;
            FailedEmail = dbNotification.FailedEmail;
        }
    }
}