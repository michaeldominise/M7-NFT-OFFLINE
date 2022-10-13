using System.Collections;
using System.Collections.Generic;
using M7.Match;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using M7.GameData;
using M7.GameRuntime;
using UnityEngine.UI.Extensions;

public class ComboIndicatorUI : MonoBehaviour {
    //[SerializeField] Text titleLabel;
    [SerializeField] TextMeshProUGUI multiplierLabel;
    [SerializeField] ParticleSystem[] vfxList;
	[SerializeField] Image multiplierImage;
    [SerializeField] Image multiplierComboImage;
    [SerializeField] Color[] vfxColor;
    [SerializeField] Image bg;
    [SerializeField] Animator animator;

	[Title("Sprite Sets")]
	[SerializeField] ComboIndicatorSpriteSet _defaultSpriteSet;
	[SerializeField] ComboIndicatorSpriteSet _pvpPlayerSpriteSet;
	[SerializeField] ComboIndicatorSpriteSet _pvpEnemySpriteSet;

    RectTransform rectTransform;
    public int startComboCount => PuzzleBoardSettings.Instance.startComboCount;
    Coroutine comboCoroutine;
    int currentComboCount;
    int newComboCount;
    bool easeInFinished = true;
    bool easeOutFinished = true;
    bool easeOutCanChange = true;
    Gradient2 mi, mci;

    //Fixed positions for the multiplier combo indicator
    const float yAnchoredPositionEnemy = -300;
    const float yAnchoredPositionPlayer = 150;

    VFX_ComboTextHandler ComboTextHandler { get { return GetComponent<VFX_ComboTextHandler>(); } }
    bool isPVP { get { return LevelManager.GameMode == LevelData.GameModeType.PVP; } }
    Sprite[] multiplierSprites
    {
        get
        {
            if (isPVP)
                return TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn ? _pvpPlayerSpriteSet.multiplierSprites : _pvpEnemySpriteSet.multiplierSprites;
            else
                return _defaultSpriteSet.multiplierSprites;
        }
    }

    Sprite[] multiplierComboSprites
    {
        get
        {
            if (isPVP)
                return TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn ? _pvpPlayerSpriteSet.multiplierComboSprites : _pvpEnemySpriteSet.multiplierComboSprites;
            else
                return _defaultSpriteSet.multiplierComboSprites;
        }
    }

    string[] titleTexts = 
    {
        "Nice!",
        "Very Good",
        "So Great",
        "Awesome",
        "Perfect",
        "Too Much",
        "Totally Dope",
        "Wicked",
        "Holy Smokes!",
        "Boom!",
        "God Like!",
    };

    void Start()
    {
        PuzzleBoardManager.Instance.onMatchSwipeStarted += ResetComboCount;
        PuzzleBoardManager.Instance.onMatchSwiped += Show;
        PuzzleBoardManager.Instance.onMatchExecuted += Show;
        PuzzleBoardManager.Instance.onMatchEnded += comboData => OnEaseOutFinish();

        if (isPVP)
        {
            mi = multiplierImage.GetComponent<Gradient2>();
            mci = multiplierComboImage.GetComponent<Gradient2>();
        }

        rectTransform = GetComponent<RectTransform>();      
    }

    public void ResetComboCount()
    {
        currentComboCount = startComboCount;
        comboCoroutine = null;
        OnEaseInFinish();
        OnEaseOutCanChange();
        OnEaseOutFinish();
    }

    public void StopCombo()
    {
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
            comboCoroutine = null;
        }

        OnEaseInFinish();
        OnEaseOutCanChange();
        OnEaseOutFinish();
    }


    void Show(ComboData comboData)
    {
        if (comboData.ComboCount > 1)
            Show(comboData.ComboCount, comboData.CellChainsGroups.Count > 0 ? comboData.LastMatchedCellType : null);
    }

    [Button]
    public void Show (int comboCount, CellType tileType) 
    {
        newComboCount = Mathf.Min(comboCount, PuzzleBoardSettings.Instance.maxComboCount);
        if (comboCount < startComboCount)
            return;

        if (comboCoroutine != null)
            return;

        comboCoroutine = StartCoroutine(StartAnim());
    }

    public IEnumerator StartAnim()
    {
        while (newComboCount >= currentComboCount)
        {
            //if (isArenaMode)
            //{
            //    ChangePosition();                
            //}

            int cappedTitleIndex = Mathf.Clamp(currentComboCount, 0, titleTexts.Length - 1);
            int cappedVFXIndex = Mathf.Clamp(currentComboCount, 0, vfxList.Length - 1);
            int cappedMultiplierSpriteIndex = Mathf.Clamp(currentComboCount, 0, multiplierSprites.Length - 1);
            int cappedMultiplierComboSpriteIndex = Mathf.Clamp(currentComboCount, 0, multiplierComboSprites.Length - 1);
            int cappedVfxColor = Mathf.Clamp(currentComboCount, 0, vfxColor.Length - 1);

            //titleLabel.text = titleTexts[comboCount > titleTexts.Length - 1 ? titleTexts.Length - 1 : comboCount];
            multiplierImage.sprite = multiplierSprites[cappedMultiplierSpriteIndex];
            multiplierComboImage.sprite = multiplierComboSprites[cappedMultiplierComboSpriteIndex];
            ComboTextHandler.ComboStreaks = cappedMultiplierComboSpriteIndex;
            VFX_ComboTextHandler.Instance.ComboStreak();

            if (multiplierLabel)
                multiplierLabel.text = "<size=12.5>x</size>" + string.Format("{0}", PuzzleBoardSettings.Instance.GetComboAttackMultiplier(currentComboCount));

            if (easeOutFinished)
            {
                VFX_ComboTextHandler.Instance.ShowComboText();
                easeOutFinished = false;
            }
            yield return new WaitForSeconds(PuzzleBoardSettings.Instance.comboInterval);
            yield return new WaitWhile(() => newComboCount == currentComboCount && !easeOutFinished);
            currentComboCount++;
        }
        yield return new WaitForSeconds(1f);
        VFX_ComboTextHandler.Instance.HideComboText();
        //animator.SetTrigger("EaseOut");
        easeOutFinished = true;
        comboCoroutine = null;
    }

    public void OnEaseInFinish()
    {
        easeInFinished = true;
    }

    public void OnEaseOutCanChange()
    {
        easeOutCanChange = true;
    }

    public void OnEaseOutFinish()
    {
        easeOutFinished = true;
    }

    private void ChangePosition()
    {
        var currentTurn = TurnManager.Instance.CurrentState;
        bool isPlayerTurn = currentTurn == TurnManager.State.PlayerTurn;
        if (mi != null || mci != null)
            if (isPlayerTurn)
                mi.enabled = mci.enabled = false;
            else
                mi.enabled = mci.enabled = true;
    }
}
