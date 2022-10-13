using System;
using M7.Match;
using UnityEngine;
using UnityEngine.Events;

namespace Observer
{
    public class MatchGridTileEventListener : MonoBehaviour
    {
        public MatchGridTileEvent gEvent;
        public UnityMatchGridTileEvent response;

        private void OnEnable()
        {
            gEvent.Register(this);
        }

        private void OnDisable()
        {
            gEvent.Unregister(this);
        }

        public void OnEventOccured(MatchGridCell obj)
        {
            response?.Invoke(obj);
        }
    }
    
    [Serializable]
    public class UnityMatchGridTileEvent : UnityEvent<MatchGridCell> { }
}