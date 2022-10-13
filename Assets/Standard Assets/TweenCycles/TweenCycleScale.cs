/*
 * TweenCycleScale.cs
 * Author: Cristjan Lazar
 */

using UnityEngine;

using DG.Tweening;

public class TweenCycleScale : TweenCycleBase {

    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private Vector3 to = Vector3.one;

    protected override Tweener InitTween () {
        return rectTransform.DOScale(to, duration);
    }

    protected override void Reset () {
        rectTransform = rectTransform ?? GetComponent<RectTransform>();
    }

}


