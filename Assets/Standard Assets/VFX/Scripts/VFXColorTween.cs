using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VFXColorTween : MonoBehaviour
{
    [SerializeField] Color _in = Color.white, _out = Color.black;
    [SerializeField] bool Looping = false;
    [SerializeField] int loopCount = 1;
    [SerializeField] float duration = 1;

    void Awake()
    {
        if (Looping)
            loopCount = -1;
    }

    public void PlayFade()
    {
        if (this.GetComponent<Image>())
        {
            Image img = this.GetComponent<Image>();
            Play(img);
        }
        else
        {
            SpriteRenderer img = this.GetComponent<SpriteRenderer>();
            Play(img);
        }
    }

    /*public void Play(GameObject objImage)
    {
        if (objImage.GetComponent<Image>())
        {
            Image img = objImage.GetComponent<Image>();
            Play(img);
        }
        else
        {
            SpriteRenderer img = objImage.GetComponent<SpriteRenderer>();
            Play(img);
        }
    }

    public void Play(Transform objImage)
    {
        if (objImage.GetComponent<Image>())
        {
            Image img = objImage.GetComponent<Image>();
            Play(img);
        }
        else
        {
            SpriteRenderer img = objImage.GetComponent<SpriteRenderer>();
            Play(img);
        }
    }*/

    public void Play(SpriteRenderer img)
    {
        img.color = _in;
        img.DOBlendableColor(_out, duration).SetLoops(loopCount, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetId("Loop");
    }

    public void Play(Image img)
    {
        img.color = _in;
        img.DOBlendableColor(_out, duration).SetLoops(loopCount, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetId("Loop");
    }

    void OnDisable()
    {
        DOTween.Kill("Loop", true);
    }
}
