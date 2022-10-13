using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TweenCycleCanvas : TweenCycleBase
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float alphaTo;
    protected override Tweener InitTween()
    {
        return canvasGroup.DOFade(alphaTo, duration);
    }

    protected override void Reset()
    {
        canvasGroup = canvasGroup ?? GetComponent<CanvasGroup>();
    }
}
