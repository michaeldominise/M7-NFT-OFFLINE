using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum TransitionState
{
    In,
    Out
}
public class SceneFXTransition : MonoBehaviour
{
    /*[SerializeField] ParticleSystem _transitionFX;
    [SerializeField] Image _fillFade;
    [SerializeField] Transform _left, _right;
    [SerializeField] float _speed = 1;
    [SerializeField] Ease _ease = Ease.OutCubic;*/

    [Range(0, 1)]
    [SerializeField] float _transition = 1;
    //[SerializeField] Texture2D _easeIn, _easeOut;
    Image bg;
    Material wipe;

    Color _color = new Color(0, 0, 0, 1);
    Vector3 _zeroScale = new Vector3(0, 1, 1);
    // Start is called before the first frame update

    void Start()
    {
        bg = GetComponent<Image>();
        wipe = bg.material;

        /*var fx = _transitionFX.main;
        fx.playOnAwake = false;
        _transitionFX.Stop();
        _fillFade.transform.localScale = _zeroScale;
        _fillFade.color = _color;*/
    }
    /*public void FXTransition()
    {
        _transitionFX.transform.position = _left.position;
        _transitionFX.Play();
        _fillFade.transform.localScale = _zeroScale;
        _fillFade.color = _color;

        Sequence seq = DOTween.Sequence();

        seq.Append(_transitionFX.transform.DOMove(_right.position, _speed)
                               .SetEase(_ease)
                               .OnComplete(_transitionFX.Stop));
        seq.Join(_fillFade.transform.DOScaleX(1, _speed * 2.5F)).SetEase(Ease.Linear);
        seq.Append(_fillFade.DOFade(0, _speed / 2.5F));
    }*/

    public void Transition(Texture2D texture)
    {
        wipe.SetTexture("_Noise", texture);
    }

    /*void StopTweenFX()
    {
        _transitionFX.Stop();
    }*/

    // Update is called once per frame
    void Update()
    {
        //if (wipe != null)
        //    wipe.SetFloat("_cutoff", _transition);

		Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(c.r,c.g,c.b, 1f - _transition);
    }
}
