using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class WindowEventTrigger : MonoBehaviour
    {
        private void OnEnable() => WindowEventManager.AddTrigger(this);
        private void OnDisable() => WindowEventManager.RemoveTrigger(this);
        public override string ToString() => this == null ? "" : $"[{GetInstanceID()}]{name}";
    }
}