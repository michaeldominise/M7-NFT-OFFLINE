using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace M7.GameRuntime
{
    public class UIValue : MonoBehaviour
    {
        [SerializeField] protected UnityEvent<float> onValueSet;
        [SerializeField, FormerlySerializedAs("onValueChange")] protected UnityEvent<float> onDisplayUpdate;
        [SerializeField] protected UnityEvent<string> onStringChange;
        [SerializeField] protected UnityEvent<bool> onNonZeroValue;
        public string stringFormat = "N0";

        [ShowInInspector, ReadOnly] public float currentValue { get; protected set; }
        [ShowInInspector] protected virtual string currentString => currentValue.ToString(stringFormat);

        [Button]
        public virtual void SetValue(float value)
        {
            currentValue = value;
            onNonZeroValue?.Invoke(currentValue != 0);
            onValueSet?.Invoke(value);
            InvokeUpdateEvents();
        }

        protected virtual void InvokeUpdateEvents()
        {
            onDisplayUpdate?.Invoke(currentValue);
            onStringChange?.Invoke(currentString);
        }
    }
}