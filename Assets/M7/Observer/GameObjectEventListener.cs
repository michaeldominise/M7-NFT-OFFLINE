using System;
using UnityEngine;
using UnityEngine.Events;

namespace Observer
{
    public class GameObjectEventListener : MonoBehaviour
    {
        public GameObjectEvent gEvent;
        public UnityGameObjectEvent response;

        private void OnEnable()
        {
            gEvent.Register(this);
        }

        private void OnDisable()
        {
            gEvent.Unregister(this);
        }

        public void OnEventOccured(GameObject obj)
        {
            response?.Invoke(obj);
        }
    }
    
    [Serializable]
    public class UnityGameObjectEvent : UnityEvent<GameObject> { }
}