using System;
using Mode.Scripts.Data;
using Unity.Notifications.iOS;

namespace Mode.Scripts.Notifications
{
    public class NotificationService
    {
        private readonly DataService _dataService;
        private readonly NotificationHandler _notificationHandler;

        public event Action OnNotificationReceived;

        public NotificationService(
            NotificationHandler notificationHandler,
            DataService dataService)
        {
            _notificationHandler = notificationHandler;
            _dataService = dataService;
        }

        public void Initialize()
        {
            iOSNotificationCenter.ApplicationBadge = 0;
            _notificationHandler.OnNotification += SetNotificationData;
        }

        public void Stop()
        {
            _notificationHandler.OnNotification -= SetNotificationData;
        }

        private void SetNotificationData(iOSNotification notification)
        {
            if (notification.UserInfo.TryGetValue(StaticData.NotificationKey, out var action))
            {
                _dataService.PushLink = action;
                OnNotificationReceived?.Invoke();
            }
            
            iOSNotificationCenter.RemoveDeliveredNotification(notification.Identifier);
            iOSNotificationCenter.ApplicationBadge = 0;
        }

    }
}