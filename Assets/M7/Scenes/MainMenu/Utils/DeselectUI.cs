using M7.GameData;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using M7.GameRuntime;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace M7
{
    public class DeselectUI : MonoBehaviour, IDeselectHandler
    {
        [SerializeField] UnityEvent onDeselect;
        [SerializeField] float delay = 0.25f;


        public void OnDeselect(BaseEventData eventData) => Execute();
        async void Execute()
        {
            await Task.Delay((int)(delay * 1000));
            onDeselect?.Invoke();
        }
    }
}
