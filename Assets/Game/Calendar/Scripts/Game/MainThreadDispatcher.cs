using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Calendar.Scripts.Game
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher _instance;
        private readonly object _lockObject = new object();
        private readonly Queue<Action<object>> _actions = new Queue<Action<object>>();
        private readonly Queue<object> _actionArgs = new Queue<object>();
    
        public static MainThreadDispatcher Instance()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MainThreadDispatcher");
                _instance = go.AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    
        public void Post(Action<object> action, object state)
        {
            lock (_lockObject)
            {
                _actions.Enqueue(action);
                _actionArgs.Enqueue(state);
            }
        }
    
        private void Update()
        {
            Action<object> action = null;
            object state = null;
        
            lock (_lockObject)
            {
                if (_actions.Count > 0)
                {
                    action = _actions.Dequeue();
                    state = _actionArgs.Dequeue();
                }
            }
        
            action?.Invoke(state);
        }
    }
}