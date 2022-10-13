/*
 * TweenCyclePunchScale.cs
 * Author: Cristjan Lazar
 */

using UnityEngine;

using DG.Tweening;

public class TweenCyclePunchScale : TweenCycleBase {

    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private Vector3 to = Vector3.one;

    [SerializeField] private int vibrato = 3;

    [SerializeField] private float elasticity = 0.5f;

    protected override Tweener InitTween () {
        return rectTransform.DOPunchScale(to, duration, vibrato, elasticity);
    }

    protected override void Reset () {
        rectTransform = rectTransform ?? GetComponent<RectTransform>();
    }

}


