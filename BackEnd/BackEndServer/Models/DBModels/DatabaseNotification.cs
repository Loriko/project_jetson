using System;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseNotification
    {
        // Table Name
        public static readonly string TABLE_NAME = "notification";
        // Attributes of Alert table.
        public static readonly string NOTIFICATION_ID_LABEL = "id";
        public static readonly string ALERT_ID_LABEL = "alert_id";
        public static readonly string TRIGGER_DATETIME_LABEL = "trigger_datetime";
        public static readonly string ACKNOWLEDGED_LABEL = "acknowledged";
        public static readonly string FAILED_EMAIL_LABEL = "failed_email";
        
        public int NotificationId { get; set; }
        public int AlertId { get; set; }
        public DateTime TriggerDateTime { get; set; }
        public bool Acknowledged { get; set; }
        public bool FailedEmail { get; set; }

        public DatabaseNotification()
        {
        }
        
        public DatabaseNotification(NotificationDetails notificationDetails)
        {
            NotificationId = notificationDetails.NotificationId;
            AlertId = notificationDetails.AlertId;
            TriggerDateTime = notificationDetails.TriggerDateTime;
            Acknowledged = notificationDetails.Acknowledged;
            FailedEmail = notificationDetails.FailedEmail;
        }
    }
}