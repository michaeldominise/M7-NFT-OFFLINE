using System.Collections.Generic;
using System.Linq;
using M7.GameData;
using M7.GameData.Scripts.RPGObjects.Boosters;
using M7.GameRuntime;
using M7.Skill;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using M7.Match;
using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace M7.PuzzleBoard.Scripts.Booster
{
    public class BoosterManager: MonoBehaviour
    {
        public static BoosterManager Instance => BattleManager.Instance.BoosterManager;

        [SerializeField] private List<BoosterButton> boosterButtons;

        [SerializeField] Camera puzzleCam;
        [SerializeField] Camera particleCam;
        [SerializeField] Transform boosterButtonParent;

        [SerializeField] CanvasGroup boosterPopUpDimPanel;
        [SerializeField] TextMeshProUGUI boosterPopUpNameText;
        [SerializeField] TextMeshProUGUI boosterPopUpInfoDetailsText;
        [SerializeField] Image boosterPopUpInfoImage;

        public CanvasGroup BoosterPopUpDimPanel => boosterPopUpDimPanel;
        public TextMeshProUGUI BoosterPopUpNameText => boosterPopUpNameText;
        public TextMeshProUGUI BoosterPopUpInfoDetailsText => boosterPopUpInfoDetailsText;
        public Image BoosterPopUpInfoImage => boosterPopUpInfoImage;
        public Camera PuzzleCam => puzzleCam;
        public Camera ParticleCam => particleCam;
        public Transform BoosterButtonParent => boosterButtonParent;

        public bool isBoosterActive;

        public SkillObject activeBoosterSkillObject;
        public BoosterButton boosterBtn;
        Tweener boosterTween;

        int boosterInteractiveCount;
        public bool IsBoosterInteractive => boosterInteractiveCount >= 0;

        public void Init(Action onFinish)
        {
            int finishCount = 0;
            void InitFinish()
            {
                finishCount++;
                if (finishCount != boosterButtons.Count)
                    return;

                onFinish.Invoke();
                TurnIsInteractable(false);
            }

            foreach (var boosterButton in boosterButtons)
                boosterButton.Init(InitFinish);
        }

        public void OffToggle()
        {
            for (int i = 0; i < boosterButtons.Count; i++)
                boosterButtons[i].GetComponent<Toggle>().isOn = false;
        }

        public void TurnIsInteractable(bool state, BoosterButton ignoreButton = null)
        {
            boosterInteractiveCount += state ? 1 : -1;
            for (int i = 0; i < boosterButtons.Count; i++)
                if(ignoreButton == null || ignoreButton != boosterButtons[i])
                    boosterButtons[i].SetInteractable(IsBoosterInteractive);
        }

        //public void ThisInteractable(bool state, Toggle thisToggle)
        //{
        //    for (int i = 0; i < boosterButtons.Count; i++)
        //            boosterButtons[i].GetComponent<Toggle>().interactable = !thisToggle.GetComponent<Toggle>().interactable;
        //    thisToggle.GetComponent<Toggle>().interactable = state;
        //}

        public void SetPanels(bool state)
        {
            DimPanel(state ? 1 : 0, 0.25f);
            PuzzleCam.depth = state ? 5 : 1;
            ParticleCam.depth = state ? 6 : 2;
        }

        public void DimPanel(float targetAlpha, float duration)
        {
            if ((!boosterTween?.IsPlaying()) ?? true)
                boosterTween.Kill();
                boosterTween = BoosterPopUpDimPanel.DOFade(targetAlpha, duration);
        }

        public void ExecuteBooster(MatchGridCell cell, Action onFinish)
        {
            SkillQueueManager.Instance.AddSkillToQueue(cell, activeBoosterSkillObject, false, onFinished: () => MatchGridCellSpawner.Instance.RefreshGrid(0.1f));
            SetPanels(false);
            TurnIsInteractable(false);
            SkillQueueManager.Instance.WaitUntilIdle(onFinish);
            boosterBtn.Decrement();

        }
    }
}