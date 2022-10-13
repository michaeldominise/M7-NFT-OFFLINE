using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class UIDelayValue : UIValue
    {
        [SerializeField] float delay = 0f;
        [SerializeField] public float addDuration = 0.25f;
        [SerializeField] float decreaseDuration = 0.25f;
        [SerializeField] bool displayWholeNumber = true;

        Coroutine coroutine;

        [Button]
        public override void SetValue(float newValue)
        {
            if (displayWholeNumber)
                newValue = newValue > currentValue ? Mathf.Floor(newValue) : Mathf.Ceil(newValue);

            if (newValue == currentValue)
            {
                SetValueInstant(newValue);
                return;
            }

            onValueSet?.Invoke(newValue);
            onNonZeroValue.Invoke(true);
            if(coroutine != null)
                StopCoroutine(coroutine);

            if(gameObject.activeInHierarchy)
                coroutine = StartCoroutine(UpdateValue(newValue));
            else
                base.SetValue(newValue);
        }

        protected virtual IEnumerator UpdateValue(float newValue)
        {
            yield return new WaitForSeconds(delay);

            var time = Time.time;
            var oldValue = currentValue;
            var duration = newValue > currentValue ? addDuration : decreaseDuration;
            while (Time.time - time < duration)
            {
                var value = Mathf.Lerp(oldValue, newValue, (Time.time - time) / duration);
                if (displayWholeNumber)
                    value = newValue > oldValue ? Mathf.Floor(value) : Mathf.Ceil(value);

                if (currentValue != value)
                {
                    currentValue = value;
                    InvokeUpdateEvents();
                }

                if (currentValue == newValue)
                    yield break;
                else
                    yield return null;
            }

            currentValue = newValue;
            InvokeUpdateEvents();
            onNonZeroValue?.Invoke(currentValue != 0);
        }

        [Button]
        public virtual void SetValueInstant(float value) => base.SetValue(value);
    }
}