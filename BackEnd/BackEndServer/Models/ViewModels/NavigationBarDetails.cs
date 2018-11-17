using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace BackEndServer.Models.ViewModels
{
    public class NavigationBarDetails
    {
        public List<NotificationDetails> NotificationList { get; set; }
        public bool SignedIn { get; set; }
        public bool IsAdministrator { get; set; }

        public NavigationBarDetails()
        {
            NotificationList = new List<NotificationDetails>();
            SignedIn = false;
            IsAdministrator = false;
        }

        public int GetUnacknowledgedNotificationCount()
        {
            return NotificationList.Count(notification => !notification.Acknowledged);
        }
    }
}