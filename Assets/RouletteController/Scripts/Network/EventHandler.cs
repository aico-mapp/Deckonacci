using System;

namespace Mode.Scripts.Network
{
    public class EventHandler
    {
        public event Action OnNotificationReceived;
        public event Action OnLoadEnded;

        public void InvokeNotificationReceived()
        {
            OnNotificationReceived?.Invoke();
        }

        public void InvokeLoadEnded()
        {
            OnLoadEnded?.Invoke();
        }
    }
}