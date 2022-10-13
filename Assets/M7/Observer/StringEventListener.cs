using System;
using UnityEngine;
using UnityEngine.Events;

namespace Observer
{
    public class StringEventListener : MonoBehaviour
    {
        public StringEvent gEvent;
        public UnityStringEvent response;

        private void OnEnable()
        {
            gEvent.Register(this);
        }

        private void OnDisable()
        {
            gEvent.Unregister(this);
        }

        public void OnEventOccured(string obj)
        {
            response?.Invoke(obj);
        }
    }
    
    [Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
}