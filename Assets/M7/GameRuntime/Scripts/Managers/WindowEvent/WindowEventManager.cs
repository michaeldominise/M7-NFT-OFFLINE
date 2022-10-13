using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace M7.GameRuntime
{
    public class WindowEventManager : MonoBehaviour
    {
        static WindowEventManager Instance => GameManager.Instance.WindowEventManager;

        [ShowInInspector] public string windowEventString => string.Join("/", windowEventTriggers);
        [ShowInInspector] public List<WindowEventTrigger> windowEventTriggers { get; private set; } = new List<WindowEventTrigger>();

        public static string WindowEventString => Instance.windowEventString;
        public static void AddTrigger(WindowEventTrigger trigger) => Instance.windowEventTriggers.Add(trigger);
        public static void RemoveTrigger(WindowEventTrigger trigger) => Instance.windowEventTriggers.Add(trigger);
    }
}
