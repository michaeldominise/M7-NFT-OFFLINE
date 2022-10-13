using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelLerpFX : MonoBehaviour
{

    private void OnEnable()
    {
        ExecuteLerp(GetComponent<RectTransform>(), 1);
    }
    private void OnDisable()
    {
        ExecuteLerp(GetComponent<RectTransform>(), 0);
    }
    public void ExecuteLerp(RectTransform rectPanel, float endValue)
    {
        rectPanel.DOScaleY(endValue, 0.3f);
    }
}
