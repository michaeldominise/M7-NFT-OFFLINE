using System.Collections.Generic;
using UnityEngine;

    #if UNITY_EDITOR
using DG.Tweening;
using M7.Tools.Scripts;
    #endif

using M7.GameData;
using Sirenix.OdinInspector;
using M7.Skill;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace M7.GameRuntime
{
    public class CharacterInstance_InventoryCard : CharacterInstance_Spine, IPointerClickHandler
    {
        [SerializeField] TextMeshProUGUI adjectiveName;
        [SerializeField] TextMeshProUGUI heroName;
        [SerializeField] Image element;
        [SerializeField] Toggle isUsedToggle;
        [SerializeField] TextMeshProUGUI mint;
        [SerializeField] TextMeshProUGUI level;
        [SerializeField] TextMeshProUGUI rarity;
        [SerializeField] TextMeshProUGUI idNumber;
        [SerializeField] Image rarityBg;
        [SerializeField] Color[] rarityColor;
        [SerializeField] Sprite[] rarityImages;
        
        public override void Init(SaveableCharacterData objectData, Action onFinish)
        {
            base.Init(objectData, SetCardInfo);
            /*adjectiveName.text = SaveableData.DisplayName;
            heroName.text = "";
            rarityBg.color = Color.white;
            rarityBg.sprite = rarityImages[0];*/
        }

        public override void OnBaseRPGObjectReferenceLoaded(Action onFinish)
        {
            SetCardInfo();
            base.OnBaseRPGObjectReferenceLoaded(onFinish);
        }

        public void SetCardInfo ()
        {
            adjectiveName.text = SaveableData.GetAdjectiveName(CharacterObject.DisplayStats.DisplayName);
            heroName.text = CharacterObject.DisplayStats.DisplayName;

            element.sprite = CharacterObject.Element.DisplayGemSprite;
            element.gameObject.SetActive(true);
            rarity.text = CharacterObject.DisplayStats.Rarity.ToString();
            var rarityIndex = (int)Mathf.Log((int)CharacterObject.DisplayStats.Rarity, 2);
            rarityBg.color = rarityColor[rarityIndex];
            rarityBg.sprite = rarityImages[rarityIndex];
        }

        public override void OnPostLoadAssetReferenceLoaded()
        {
            if (CharacterObject == null)
                return;
            base.OnPostLoadAssetReferenceLoaded();

	        MainSpineInstance.GetComponentInChildren<Animator>().enabled = false;
	        if (SubSpineInstance != null)
	        {
		        SubSpineInstance.GetComponentInChildren<Animator>().enabled = false;
                if (CharacterObject.DisplayStats.SubSpine.Asset == null)
                {
                    Destroy(SubSpineInstance.gameObject);
                }
	        }
        }

        public override void RefreshNonAssetReferenceDisplay()
        {
            base.RefreshNonAssetReferenceDisplay();

            adjectiveName.gameObject.SetActive(!SaveableCharacterData.IsNonNFT);
            idNumber.gameObject.SetActive(!SaveableCharacterData.IsNonNFT);
            isUsedToggle.isOn = PlayerDatabase.Teams.IsHeroInAnyTeam(SaveableCharacterData.InstanceID);
            mint.text = $"Mint {SaveableCharacterData.MintCount} / {SaveableCharacterData.MintMax}";
	        level.text = $"Lv {SaveableCharacterData.Level}";
            idNumber.text = "#" + $"{int.Parse(SaveableCharacterData.InstanceID):0000000}";
        }

        public override void CleanInstance()
        {
            base.CleanInstance();
            heroName.text = "";
            element.sprite = null;
            isUsedToggle.isOn = false;
            mint.text = "";
            level.text = "";
            rarity.text = "";

            idNumber.text = "";
            rarityBg.color = Color.white;
            rarityBg.sprite = rarityImages[0];
        }
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            MenuTeamSelector_EditTeam e = FindObjectOfType<MenuTeamSelector_EditTeam>();
            if (e != null)
                e.InitDisplayValues();

            CharacterInfoManager.isReadyLevelUp = true;
            SaveableData.IsDirty = true;
            onClickInstance?.Invoke(SaveableData);
        }
    }
}
