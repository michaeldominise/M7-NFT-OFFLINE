using System.Collections.Generic;
using UnityEngine;

namespace Observer
{
    [CreateAssetMenu(fileName = "New Button Click Game Event", menuName = "Observer/Button Click Game Event", order = 4)]
    public class ButtonClickEvent : ScriptableObject
    {
        private List<ButtonClickEventListener> eventListeners = new List<ButtonClickEventListener>();

        public void Register(ButtonClickEventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public void Unregister(ButtonClickEventListener eventListener)
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