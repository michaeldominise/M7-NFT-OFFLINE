using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Observer
{
    public class EventListener : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private List<Object> objectWithEvents; 
#endif
        
        public Event gEvent;
        public UnityEvent response;

        private void OnEnable()
        {
            gEvent.Register(this);
        }

        private void OnDisable()
        {
            gEvent.Unregister(this);
        }

        public void OnEventOccured()
        {
            response?.Invoke();
        }
    }
}