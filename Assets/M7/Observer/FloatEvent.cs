using System.Collections.Generic;
using UnityEngine;

namespace Observer
{
    [CreateAssetMenu(fileName = "New Game Event float parameter", menuName = "Observer/Game Event, float parameter", order = 4)]
    public class FloatEvent : ScriptableObject
    {
        private List<FloatEventListener> eventListeners = new List<FloatEventListener>();

        public void Register(FloatEventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public void Unregister(FloatEventListener eventListener)
        {
            eventListeners.Remove(eventListener);
        }

        public void Occured(float floatVal)
        {
            for (var index = 0; index < eventListeners.Count; index++)
            {
                eventListeners[index].OnEventOccured(floatVal);
            }
        }
    }
}