using System.Collections.Generic;
using M7.Match;
using UnityEngine;

namespace Observer
{
    [CreateAssetMenu(fileName = "New Game Event MatchGridTile parameter", menuName = "Observer/Game Event, MatchGridTile parameter", order = 5)]
    public class MatchGridTileEvent : ScriptableObject
    {
        private List<MatchGridTileEventListener> eventListeners = new List<MatchGridTileEventListener>();

        public void Register(MatchGridTileEventListener eventListener)
        {
            eventListeners.Add(eventListener);
        }

        public void Unregister(MatchGridTileEventListener eventListener)
        {
            eventListeners.Remove(eventListener);
        }

        public void Occured(MatchGridCell strVal)
        {
            for (var index = 0; index < eventListeners.Count; index++)
            {
                eventListeners[index].OnEventOccured(strVal);
            }
        }
    }
}