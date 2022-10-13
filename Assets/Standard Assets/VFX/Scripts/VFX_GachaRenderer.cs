using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class VFX_GachaRenderer : MonoBehaviour
{
    [SerializeField] GameObject _InfoPanel;
    [SerializeField] float startScale = .5F, fadeDuration = .5F;

    Camera cam { get { return GetComponentInChildren<Camera>(); } }
    RawImage rawImg { get { return GetComponentInChildren<RawImage>(); } }
    const int size = 2048;
    RenderTexture rt;

    // Start is called before the first frame update
    void Awake()
    {
        rt = new RenderTexture(size, size, 16, RenderTextureFormat.ARGB32);
        rt.Create();

        cam.targetTexture = rt;
        rawImg.texture = rt;
    }

    [Button("Show Gacha")]
    public void RenderGachaImage()
    {
        DOTween.Kill(this);
        //initial setting
        _InfoPanel.SetActive(false);
        rawImg.color = Color.clear;
        rawImg.transform.localScale = Vector3.one * startScale;

        rawImg.transform.DOScale(Vector3.one, fadeDuration);
        rawImg.DOColor(Color.white, fadeDuration).OnComplete(delegate ()
        {
            _InfoPanel?.SetActive(true);
        });
    }

}
