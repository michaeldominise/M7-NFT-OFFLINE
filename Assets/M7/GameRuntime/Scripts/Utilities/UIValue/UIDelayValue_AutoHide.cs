using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class UIDelayValue_AutoHide : UIDelayValue
    {
        [SerializeField] protected List<GameObject> hideGameObjects;
        [SerializeField] float hideDelay = 2;

        Coroutine coroutine;

        [Button]
        public override void SetValue(float newValue)
        {
            if (newValue == currentValue)
                return;

            onNonZeroValue.Invoke(true);
            if(coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(UpdateValue(newValue));
        }

        protected override IEnumerator UpdateValue(float newValue)
        {
            yield return base.UpdateValue(newValue);
            yield return new WaitForSeconds(hideDelay);
            hideGameObjects.ForEach(x => x.SetActive(false));
        }
    }
}