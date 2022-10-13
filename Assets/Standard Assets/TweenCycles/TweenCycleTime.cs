/*
 * TweenCycleTime.cs
 * Author: Cristjan Lazar
 */

using UnityEngine;

using DG.Tweening;

public class TweenCycleTime : TweenCycleBase {

    [SerializeField] private float to = 1f;

    float counter = 0f;

    protected override Tweener InitTween () {
        return DOTween.To(() => counter, t => counter = t, to, duration);
    }

    protected override void Reset () {
        // Blank
    }

}

