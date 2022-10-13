using System.Collections.Generic;
using UnityEngine;

namespace Observer
{
    [CreateAssetMenu(fileName = "New Game Event string parameter", menuName = "Observer/Game Event, string parameter", order = 4)]
    public class StringEvent : ScriptableObject
    {
        private List<StringEventListener> eventListeners = new List<StringEventListener>();

        public void Register(StringEventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public void Unregister(StringEventListener eventListener)
        {
            eventListeners.Remove(eventListener);
        }

        public void Occured(string strVal)
        {
            for (var index = 0; index < eventListeners.Count; index++)
            {
                eventListeners[index].OnEventOccured(strVal);
            }
        }
    }
}