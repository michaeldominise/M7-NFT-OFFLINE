using System;
using UnityEngine;
using UnityEngine.Events;

namespace Observer
{
    public class BoolEventListener : MonoBehaviour
    {
        public BoolEvent gEvent;
        public UnityBoolEvent response;

        private void OnEnable()
        {
            gEvent.Register(this);
        }

        private void OnDisable()
        {
            gEvent.Unregister(this);
        }

        public void OnEventOccured(bool obj)
        {
            response?.Invoke(obj);
        }
    }
    
    [Serializable]
    public class UnityBoolEvent : UnityEvent<bool> {}
}