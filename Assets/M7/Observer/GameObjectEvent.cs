using System.Collections.Generic;
using UnityEngine;

namespace Observer
{
    [CreateAssetMenu(fileName = "New GameObject Event", menuName = "Observer/GameObject Event", order = 6)]
    public class GameObjectEvent : ScriptableObject
    {
        private List<GameObjectEventListener> eventListeners = new List<GameObjectEventListener>();

        public void Register(GameObjectEventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public void Unregister(GameObjectEventListener eventListener)
        {
            eventListeners.Remove(eventListener);
        }

        public void Occured(GameObject strVal)
        {
            for (var index = 0; index < eventListeners.Count; index++)
            {
                eventListeners[index].OnEventOccured(strVal);
            }
        }
    }
}