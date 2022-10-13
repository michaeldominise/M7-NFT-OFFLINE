using M7.GameData.Scripts.RPGObjects.Boosters;
using M7.Skill;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using M7.GameRuntime;
using M7.GameData;
using System;
using M7.GameRuntime.Scripts.PlayfabCloudscript;

namespace M7.PuzzleBoard.Scripts.Booster
{
    public class BoosterButton :  BaseRPGObjectInstance<SaveableBoosterData>
    {
#if UNITY_EDITOR
        [SerializeField] private bool isFree;  
#endif

        [SerializeField] string masterID;
        [SerializeField] Toggle toggle;
        [SerializeField] Image iconImage;
        [SerializeField] Image highlightPanel;

        [SerializeField] Sprite boosterNotHighlight;
        [SerializeField] Sprite boosterHighlight;

        [SerializeField] SkillObject skillObject;
        [SerializeField] TextMeshProUGUI amountText;

        public BoosterObject BoosterObject => AssetReference.Asset as BoosterObject;
        bool allowToggle = true;

        public void Init(Action onFinish) => Init(PlayerDatabase.Inventories.Boosters.FindItem(masterID), onFinish);
        public override void OnBaseRPGObjectReferenceLoaded(Action onFinish)
        {
            iconImage.sprite = BoosterObject.DisplayStats.Icon;
            base.OnBaseRPGObjectReferenceLoaded(onFinish);
        }

        public override void RefreshNonAssetReferenceDisplay() => amountText.text = $"{ SaveableData.Amount}";

        // for editor only
        private bool FreeUse()
        {
#if UNITY_EDITOR
            if (isFree)
                return true;
#endif
            return false;
        }

        public void SetInteractable(bool value)
        {
            value = SaveableData.Amount > 0 && value;

            toggle.interactable = value;
            toggle.enabled = value;
        }

        public void BoosterClick(bool isActive)
        {
            Debug.Log($"Booster Click {gameObject.name}");
            CheckeAllChildBtns();
            BoosterManager.Instance.DimPanel(isActive ? 1 : 0, BoosterManager.Instance.BoosterPopUpDimPanel.alpha == 1 ? 0 : 0f);
            BoosterManager.Instance.PuzzleCam.depth = isActive ? 6 : 1;
            BoosterManager.Instance.ParticleCam.depth = isActive ? 7 : 2;

            if (isActive)
            {
                highlightPanel.sprite = boosterHighlight;
                BoosterManager.Instance.activeBoosterSkillObject = skillObject;
                BoosterManager.Instance.boosterBtn = GetComponent<BoosterButton>();
                BoosterManager.Instance.isBoosterActive = true;
                BoosterManager.Instance.TurnIsInteractable(false, this);
            }
            else
            {
                highlightPanel.sprite = boosterNotHighlight;
                BoosterManager.Instance.activeBoosterSkillObject = null;
                BoosterManager.Instance.isBoosterActive = false;
                BoosterManager.Instance.TurnIsInteractable(true);
            }
        }

        void CheckeAllChildBtns()
        {
            foreach(Transform boosterBtn in BoosterManager.Instance.BoosterButtonParent)
            {
                boosterBtn.GetChild(0).transform.GetComponent<Image>().sprite = boosterNotHighlight;
            }
        }
        void SetInfo()
        {
            BoosterManager.Instance.BoosterPopUpNameText.text = BoosterObject.DisplayStats.DisplayName;
            BoosterManager.Instance.BoosterPopUpInfoDetailsText.text = BoosterObject.DisplayStats.DescriptionText;
            BoosterManager.Instance.BoosterPopUpInfoImage.sprite = BoosterObject.DisplayStats.Icon;
        }

        public void ToggleCheck()
        {
            toggle.GetComponent<BoosterButton>().BoosterClick(toggle.isOn);
            SetInfo();
        }

        public void Decrement()
        {
            SaveableData.Decrement();
            RefreshNonAssetReferenceDisplay();
            PlayFabFunctions.PlayFabCallFunction("SetBoosterData", false, "", masterID, SetBoosterCallback);
            //Playfab Callback Here
        }

        void SetBoosterCallback(ExecuteResult result)
        {
            Debug.Log($"Set Booster: {result}");
        }
    }
}