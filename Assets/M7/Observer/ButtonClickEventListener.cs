using UnityEngine;
using UnityEngine.Events;

namespace Observer
{
    public class ButtonClickEventListener : MonoBehaviour
    {
        public ButtonClickEvent gEvent;
        public UnityEvent response;

        private void OnEnable()
        {
            gEvent.Register(this);
        }

        private void OnDisable()
        {
            gEvent.Unregister(this);
        }

        public void OnEventOccured()
        {
            response?.Invoke();
        }
    }
}