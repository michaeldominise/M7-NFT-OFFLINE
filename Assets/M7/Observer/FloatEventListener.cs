using System;
using UnityEngine;
using UnityEngine.Events;

namespace Observer
{
    public class FloatEventListener : MonoBehaviour
    {
        public FloatEvent gEvent;
        public UnityFloatEvent response;

        private void OnEnable()
        {
            gEvent.Register(this);
        }

        private void OnDisable()
        {
            gEvent.Unregister(this);
        }

        public void OnEventOccured(float obj)
        {
            response?.Invoke(obj);
        }
    }
    
    [Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }
}