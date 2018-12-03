using System.Collections.Generic;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.ViewModels;

namespace BackEndServer.Services.AbstractServices
{
    public interface AbstractNotificationService
    {
        List<NotificationDetails> GetNotificationsForUser(int userId);
        NotificationDetails GetNotificationDetailsById(int notificationId);
        void AcknowledgeNotification(int notificationId);
        bool IsNotificationAcknowledged(int notificationId);
        bool DoesNotificationNeedImage(DatabaseAlert dbAlert, DatabaseNotification dbNotification);
    }
}