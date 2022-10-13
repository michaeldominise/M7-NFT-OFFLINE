/*
 * TweenCycleImageFill.cs
 * Author: Cristjan Lazar
 */

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class TweenCycleImageFill : TweenCycleBase {

    [SerializeField] private Image image;

    [SerializeField] private float to = 1f;

    protected override Tweener InitTween () {
        return DOTween.To(() => image.fillAmount, f => image.fillAmount = f, to, duration);
    }

    protected override void Reset () {
        image = image ?? GetComponent<Image>();
    }

}

