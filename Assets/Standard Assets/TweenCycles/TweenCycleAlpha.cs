/*
 * TweenCycleAlpha.cs
 * Author: Cristjan Lazar
 */

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class TweenCycleAlpha : TweenCycleBase {

    [SerializeField] private Image image;

    [SerializeField] private Color to = Color.white;

    protected override Tweener InitTween () {
        return image.DOColor(to, duration);
    }

    protected override void Reset () {
        image = image ?? GetComponent<Image>();
    }

}

