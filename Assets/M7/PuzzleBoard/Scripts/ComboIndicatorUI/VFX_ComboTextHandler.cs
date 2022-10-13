using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DarkTonic.MasterAudio;
using TMPro;
using M7.GameRuntime;

public class VFX_ComboTextHandler : MonoBehaviour
{
    public static VFX_ComboTextHandler Instance { get; private set; }

    [System.Serializable]
    struct ComboText
    {
        public Transform MainComp, Text;
        public Image glowImage;
        public Vector2 defaultPos, maxComboOffset;
        public float speed;
    }

    [System.Serializable]
    struct AtkMultiplier
    {
        public Color colorValue;
    }

    [Header("Combo Streak")]
    [SerializeField] ComboText _ComboValue;
    [SerializeField] ComboText _XValue;
    [SerializeField] float _delayedEffect;
    [Header("Attack Multiplier")]
    [SerializeField] TextMeshProUGUI MultiplierText;
    [SerializeField] AtkMultiplier[] MultilplierValues;
    [Header("VFX Parameters")]
    [SerializeField] ParticleSystem maxComboFX;

    Vector3 normalizeScale = Vector3.one, gradScale;
    float SFXPitch;
    bool ComboActive;

    CanvasGroup MainCompGroup(Transform obj) { return obj.GetComponent<CanvasGroup>(); }
    Sprite SpriteReference(Transform ImageRef) { return ImageRef.GetComponent<Image>().sprite; }
    ComboIndicatorUI ComboIndicator { get { return GetComponent<ComboIndicatorUI>(); } }
    public int ComboStreaks { get; set; }
    RectTransform m_rect(Transform obj) { return obj.GetComponent<RectTransform>(); }
    bool isPVP { get { return LevelManager.GameMode == M7.GameData.LevelData.GameModeType.PVP; } }
    bool isPlayerTurn { get { return TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn; } }
    bool isBlitzRaid { get { return false; } }

    //const float anchoredPosition = 111;
    const float fadeIn = 1, fadeOut = 0;
    //const float blitzAnchoredPosition = 111;
    void Awake()
    {
        Instance = this;
        Normalized();
        m_rect(transform).pivot = Vector2.one * .5F;
        SetPosition();
    }

    //for arena turn header positioning
    void SetPosition()
    {
        //float yValue = isBlitzRaid ? blitzAnchoredPosition : anchoredPosition;
        
        //if (isArena)
        //    yValue = isPlayerTurn ? Mathf.Abs(anchoredPosition) : -Mathf.Abs(anchoredPosition);

        //m_rect(transform).anchoredPosition = new Vector2(0, yValue);
    }

    public void ShowComboText()
    {
        if (!ComboActive)
            StartCoroutine(DelayedTiming());
    }

    public void HideComboText()
    {
        maxComboFX.Stop();
        FadeInOutText(fadeOut);
    }

    public void ComboStreak()
    {
        if (ComboActive)
        {
            GradualScaleEffect();
            GlowEffect();
            IncreasePitch();
            MultiplierTextIncrease();
        }
    }

    IEnumerator DelayedTiming()
    {
        SetPosition();
        yield return new WaitForSeconds(_delayedEffect);

        ComboActive = true;
        FadeInOutText(fadeIn);
        _ComboValue.MainComp.gameObject.SetActive(true);
        _XValue.MainComp.gameObject.SetActive(true);
        GlowEffect();
        //SFXManager.PlaySound(audioclip, audioclip, null, SFXPitch);
        //MasterAudio.PlaySound(audioclip, 1, SFXPitch);
        StopAllCoroutines();
    }

    void FadeInOutText(float to)
    {
        MainCompGroup(_ComboValue.MainComp).DOFade(to, _ComboValue.speed);
        MainCompGroup(_XValue.MainComp).DOFade(to, _ComboValue.speed)
            .OnComplete(delegate ()
            {
                if (to == 0)
                    Normalized();
            });
    }

    void GradualScaleEffect()
    {
        if (ComboStreaks < MultilplierValues.Length - 1)
        {
            gradScale = _ComboValue.MainComp.localScale + Vector3.one * .05F;
            _ComboValue.MainComp.localScale += gradScale * .125F;
            MultiplierText.transform.localScale += gradScale * .1F;
        }
        MultiplierText.transform.DOScale(gradScale, _XValue.speed);
        _ComboValue.MainComp.DOScale(gradScale, _XValue.speed).SetEase(Ease.OutBack);
    }

    void GlowEffect()
    {
        _ComboValue.glowImage.sprite = SpriteReference(_ComboValue.Text);
        _XValue.glowImage.sprite = SpriteReference(_XValue.Text);
        _ComboValue.glowImage.transform.localScale = normalizeScale;
        _ComboValue.glowImage.color = _XValue.glowImage.color = new Color(1, 1, 1, 1);
        _ComboValue.glowImage.DOFade(0, _XValue.speed);
        _XValue.glowImage.DOFade(0, _XValue.speed);
    }

    void MultiplierTextIncrease()
    {
        if (ComboStreaks >= MultilplierValues.Length - 1)
            MaxCombo();

        MultiplierText.color = MultilplierValues[ComboStreaks].colorValue;
    }

    void IncreasePitch()
    {
        if (ComboStreaks < MultilplierValues.Length - 1)
            SFXPitch += .1F;
        //SFXManager.PlaySound(audioclip, audioclip, null, SFXPitch);
        //MasterAudio.PlaySound(audioclip, 1, SFXPitch);
    }

    void MaxCombo()
    {
        maxComboFX.Play();
        m_rect(_ComboValue.Text).anchoredPosition = _ComboValue.maxComboOffset;
    }

    void Normalized()
    {
        ComboActive = false;
        DOTween.Kill(this);

        CanvasGroup comboValueGroup = MainCompGroup(_ComboValue.Text);
        CanvasGroup xValueGroup = MainCompGroup(_XValue.Text);

        _ComboValue.MainComp.gameObject.SetActive(false);
        _XValue.MainComp.gameObject.SetActive(false);
        _ComboValue.MainComp.localScale = normalizeScale;
        m_rect(_ComboValue.Text).anchoredPosition = _ComboValue.defaultPos;
        MultiplierText.transform.localScale = normalizeScale;
        if(comboValueGroup != null)
            comboValueGroup.alpha = fadeOut;
        if(xValueGroup != null)
            xValueGroup.alpha = fadeOut;

        SFXPitch = 1;
        MultiplierText.transform.localScale = normalizeScale;
        MultiplierText.color = MultilplierValues[0].colorValue;
    }
    
}
