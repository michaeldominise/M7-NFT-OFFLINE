using System.Collections.Generic;
using UnityEngine;

namespace Observer
{
    [CreateAssetMenu(fileName = "New Game Event bool parameter", menuName = "Observer/Game Event, bool parameter", order = 4)]
    public class BoolEvent : ScriptableObject
    {
        private List<BoolEventListener> eventListeners = new List<BoolEventListener>();

        public void Register(BoolEventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public void Unregister(BoolEventListener eventListener)
        {
            eventListeners.Remove(eventListener);
        }

        public void Occured(bool strVal)
        {
            for (var index = 0; index < eventListeners.Count; index++)
            {
                eventListeners[index].OnEventOccured(strVal);
            }
        }
    }
}