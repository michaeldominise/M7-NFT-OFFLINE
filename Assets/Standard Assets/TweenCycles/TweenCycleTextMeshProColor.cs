/*
 * TweenCycleTextMeshProColor.cs
 * Author: Cristjan Lazar
 */

using DG.Tweening;
using UnityEngine;

using TMPro;

public class TweenCycleTextMeshProColor : TweenCycleBase {

    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Color to;

    protected override Tweener InitTween () {
        return text.DOColor(to, duration);
    }

    protected override void Reset () {
        text = text ?? GetComponent<TextMeshProUGUI>();
    }

}
