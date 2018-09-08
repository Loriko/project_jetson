using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;
using BackEndServer.Services.AbstractServices;
using Microsoft.CodeAnalysis;

namespace BackEndServer.Services
{
    public class NotificationService : AbstractNotificationService
    {
        private readonly IDatabaseQueryService _databaseQueryService;

        public NotificationService(IDatabaseQueryService databaseQueryService)
        {
            _databaseQueryService = databaseQueryService;
        }
        
        public List<NotificationDetails> GetNotificationsForUser(int userId)
        {
            List<DatabaseNotification> dbNotifications = _databaseQueryService.GetNotificationsForUser(userId);
            List<NotificationDetails> notifications = new List<NotificationDetails>();
            List<int> alertIds = new List<int>();
            foreach (DatabaseNotification dbNotification in dbNotifications)
            {
                notifications.Add(new NotificationDetails(dbNotification));
                alertIds.Add(dbNotification.AlertId);
            }
            List<DatabaseAlert> dbAlerts = _databaseQueryService.GetAlertsById(alertIds);
            foreach (DatabaseAlert alert in dbAlerts)
            {
                notifications.FindAll(notification => notification.AlertId == alert.AlertId).ForEach(notification => notification.Alert = new AlertDetails(alert));
            }

            return notifications;
        }

        public NotificationDetails GetNotificationDetailsById(int notificationId)
        {
            NotificationDetails notification = new NotificationDetails(_databaseQueryService.GetNotificationById(notificationId));
            notification.Alert = new AlertDetails(_databaseQueryService.GetAlertById(notification.AlertId));
            notification.Alert.Camera = new CameraDetails(_databaseQueryService.GetCameraById(notification.Alert.CameraId));
            notification.Alert.Camera.Location = new LocationDetails(_databaseQueryService.GetLocationById(notification.Alert.Camera.LocationId));
            return notification;
        }

        public void AcknowledgeNotification(int notificationId)
        {
            _databaseQueryService.AcknowledgeNotification(notificationId);
        }

        public bool IsNotificationAcknowledged(int notificationId)
        {
            DatabaseNotification dbNotification = _databaseQueryService.GetNotificationById(notificationId);
            return dbNotification.Acknowledged;
        }
    }
}