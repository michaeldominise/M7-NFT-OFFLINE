using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class RewardImageTweener : MonoBehaviour
{
    [Header("Main Parameter")]
    [SerializeField] bool isWorld = false, isStandalone = false;
    [SerializeField] Transform _icon, _startPos, _endPos, _button;
    [SerializeField] GameObject _bodyText, _itemCount;
    [SerializeField] ParticleSystem[] _endFX;
    [Header("Tween Parameter")]
    [SerializeField] Vector3 scaleIn = Vector3.one * 4;
    [SerializeField] Vector3 scaleOut = Vector3.one;
    [SerializeField] float _jumpPower = 5;
    [SerializeField] Ease _setEase = Ease.Linear;
    [SerializeField] float _durationOfTween = 1;
    [Header("Stand Alone Parameter")]
    [EnableIf("isStandalone", true)] [SerializeField] float _delay = 0;
    [EnableIf("isStandalone", true)] [SerializeField] bool canShowButton = false;
    
    Sequence anim;
    Image _container;
    RectTransform m_rect(Transform obj) { return obj.GetComponent<RectTransform>(); }

    public Transform ObjectToTarget(GameObject obj)
    {
        if (obj != null)
            if (_icon == null)
                _icon = obj.transform;
        
        return _icon;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        DOTween.Kill(this);
        if (_endFX != null)
            foreach (var pfx in _endFX)
            {
                var fx = pfx.main;
                fx.playOnAwake = false;
            }

        if (isStandalone)
            TweenStart(_delay);
    }

    public void TweenStart(float value)
    {
        anim = DOTween.Sequence();
        anim.Restart();
        _container = _endPos.gameObject.GetComponent<Image>();

        TweenIcon(value);
    }

    void TweenSetup()
    {
        //set the start position and alpha of animation sequence
        if (_icon != null)
        {
            _icon.localScale = Vector3.zero;
            if (_startPos != null)
                if (!isWorld)
                    _icon.localPosition = _startPos.localPosition;
                else
                    _icon.position = _startPos.position;
            else
                m_rect(_icon).anchoredPosition = Vector2.zero;
        }

        if (_button != null)
            _button.localScale = _icon.localScale = Vector3.zero;

        Color col = new Color(1, 1, 1, 0);
        if (_container != null)
            _container.color = col;

        TextVisibilty(_bodyText, col.a);
        TextVisibilty(_itemCount, col.a);
    }

    void TweenIcon(float _interval)
    {
        TweenSetup();
        //scale popup icon
        anim.PrependInterval(_interval); //assigned by MessageBoxPopup if has multiple icon to tween
        anim.Append(_icon.DOScale(scaleIn, .3F).SetEase(Ease.OutBack));
        anim.AppendCallback(PlayEndFX);

        //jump animation to end position
        anim.AppendInterval(.7F);
        anim.Append(_icon.DOLocalJump(m_rect(_endPos).anchoredPosition, _jumpPower * 100, 1, _durationOfTween).SetEase(_setEase));
        if (_container != null)
            anim.Join(_container.DOFade(1, .2F));
        anim.Join(_icon.DOScale(scaleOut, _durationOfTween).SetEase(Ease.InBack));

        //Fade Text
        if (_bodyText != null)
            anim.Join(FadeText(_bodyText));

        if (_itemCount != null)
            anim.Join(FadeText(_itemCount));

        if (_button != null)
            if (canShowButton)
                anim.OnComplete(ShowButton);
    }

    Tween FadeText(GameObject obj)
    {
        Tween fade = null;
        if (obj.GetComponent<Text>())
            fade = obj.GetComponent<Text>().DOFade(1, .4F);
        else if (obj.GetComponent<TextMeshProUGUI>())
            fade = obj.GetComponent<TextMeshProUGUI>().DOFade(1, .4F);

        return fade;
    }

    void TextVisibilty(GameObject obj, float alpha)
    {
        Color col = new Color(1, 1, 1, alpha);
        if (obj != null)
            if (obj.GetComponent<Text>())
                obj.GetComponent<Text>().color = col;
            else if (obj.GetComponent<TextMeshProUGUI>())
                obj.GetComponent<TextMeshProUGUI>().color = col;
    }

    void PlayEndFX()
    {
        if (_endFX != null)
            foreach (var pfx in _endFX)
            {
                pfx.transform.SetParent(_icon.transform, false);
                pfx.Play();
            }
    }

    public void OnSkipSequence()
    {
        anim.Complete();
        _icon.localPosition = _endPos.localPosition;
        _icon.localScale = Vector3.one;
        
        Color col = new Color(1, 1, 1, 1);
        if (_container != null)
            _container.color = col;

        TextVisibilty(_bodyText, 1);
        TextVisibilty(_itemCount, 1);
        ShowButton();
    }

    public float TweenDuration
    {
        get { return anim.Duration(); }
    }

    public void ShowButton()
    {
        _button.DOScale(Vector3.one, .2F).SetEase(Ease.OutBack);
        anim.Complete();
    }
}
