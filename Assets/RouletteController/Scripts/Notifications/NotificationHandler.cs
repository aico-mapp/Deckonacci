using System;
using System.Collections.Generic;
using Unity.Notifications.iOS;
using UnityEngine;

namespace Mode.Scripts.Notifications
{
    public class NotificationHandler : MonoBehaviour
    {
        public event Action<iOSNotification> OnNotification;
    
        private readonly HashSet<string> _processedNotifications = new ();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            var query = iOSNotificationCenter.QueryLastRespondedNotification();
            if (query.State != QueryLastRespondedNotificationState.HaveRespondedNotification) return;

            var notification = query.Notification;
            if (notification == null) return;

            var identifier = notification.Identifier;
            if (!_processedNotifications.Add(identifier)) return;

            OnNotification?.Invoke(notification);
        }
    }
}