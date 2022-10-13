using System.Collections.Generic;
using UnityEngine;

namespace Observer
{
    [CreateAssetMenu(fileName = "New Game Event", menuName = "Observer/Game Event", order = 4)]
    public class Event : ScriptableObject
    {
        private List<EventListener> eventListeners = new List<EventListener>();

        public void Register(EventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public void Unregister(EventListener eventListener)
        {
            eventListeners.Remove(eventListener);
        }

        public void Occured()
        {
            for (var index = 0; index < eventListeners.Count; index++)
            {
                eventListeners[index].OnEventOccured();
            }
        }
    }
}