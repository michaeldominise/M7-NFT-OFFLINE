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
    public class GemInstance_InventoryCard : BaseGemInstance
    {
        [SerializeField] Image boosterImage;
        [SerializeField] TextMeshProUGUI displayName;

        public override void OnBaseRPGObjectReferenceLoaded(Action onFinish)
        {
            base.OnBaseRPGObjectReferenceLoaded(onFinish);
            displayName.text = GemObject.DisplayStats.DisplayName;
            //boosterImage.sprite = GemObject.DisplayStats.Icon.Asset as Sprite;
        }

        public override void RefreshNonAssetReferenceDisplay()
        {
            base.RefreshNonAssetReferenceDisplay();
            //amount.text = $"QTY {SaveableGemData}";
        }

        public override void CleanInstance()
        {
            base.CleanInstance();
            boosterImage.sprite = null;
            displayName.text = "";
            //amount.text = "";
        }
    }
}
