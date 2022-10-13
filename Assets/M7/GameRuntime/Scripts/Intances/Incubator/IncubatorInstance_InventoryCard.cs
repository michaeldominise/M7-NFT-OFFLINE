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

namespace M7.GameRuntime
{
    public class IncubatorInstance_InventoryCard : BaseIncubatorInstance
    {
        [SerializeField] Image incubatorImage;
        [SerializeField] TextMeshProUGUI rarity;
        [SerializeField] TextMeshProUGUI idNumber;
        [SerializeField] Image rarityBg;
        [SerializeField] Color[] rarityColor;
        [SerializeField] Sprite[] rarityImages;

        public override void OnBaseRPGObjectReferenceLoaded(Action onFinish)
        {
            base.OnBaseRPGObjectReferenceLoaded(onFinish);
            incubatorImage.sprite = IncubatorObject.DisplayImage;
            rarity.text = IncubatorObject.Rarity.ToString();
            var rarityIndex = (int)Mathf.Log((int)IncubatorObject.Rarity, 2);
            rarityBg.color = rarityColor[rarityIndex];
            rarityBg.sprite = rarityImages[rarityIndex];
        }

        public override void RefreshNonAssetReferenceDisplay()
        {
            base.RefreshNonAssetReferenceDisplay();
            idNumber.text = "#" + $"{int.Parse(SaveableData.InstanceID):0000000}";
        }

        public override void CleanInstance()
        {
            base.CleanInstance();
            incubatorImage.sprite = null;
            rarity.text = "";
            idNumber.text = "";
            rarityBg.color = Color.white;
            rarityBg.sprite = rarityImages[0];
        }
    }
}
