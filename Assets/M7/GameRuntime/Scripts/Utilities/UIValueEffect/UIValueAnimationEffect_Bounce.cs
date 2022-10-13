using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class UIValueAnimationEffect_Bounce : UIValueAnimationEffect
    {
        [SerializeField] AnimationCurve bounceCurve;
        [SerializeField] float bounceIntensity = 1f;
        [SerializeField] float duration = 0.1f;
        [SerializeField] bool lockZ = true;
        [SerializeField] float interval = 0f;

        float startTime;

        public override void OnValueUpdate(float value) => startTime = startTime + interval < Time.time ? Time.time : startTime;
        private void Update() => transform.localScale = Vector3.one + new Vector3 (1, 1, lockZ ? 0 : 1) * bounceIntensity * bounceCurve.Evaluate((Time.time - startTime) / duration);
    }
}