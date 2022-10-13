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
using M7.CDN.Addressable;
using UnityEngine.EventSystems;

namespace M7.GameRuntime
{
    public class ChapterInstance_Map : BaseChapterInstance, IPointerClickHandler
    {
        [SerializeField] Image mapBanner;
        [SerializeField] TextMeshProUGUI[] locationNames;
        [SerializeField] TextMeshProUGUI stageLabel;

        public override void Init(ChapterData objectData, Action onFinish)
        {
            if (this.ObjectData == objectData)
                return;

            CleanInstance();
            ObjectData = objectData;

            BannerAssetReference.LoadAssetAsync(result =>
            {
                AddressableAssetDisposeManager.AddDisposableAssetReference(this);
                OnBaseRPGObjectReferenceLoaded();
                onFinish?.Invoke();
            });
            RefreshNonAssetReferenceDisplay();
        }

        public override void RefreshNonAssetReferenceDisplay()
        {
            foreach(var locationName in locationNames)
                locationName.text = ChapterData.DisplayName;
            stageLabel.text = $"{ChapterData.FirstLevelData.DisplayName}-{ChapterData.LastLevelData.MasterID.Replace("Stage_", "")}";
        }

        public override void OnBaseRPGObjectReferenceLoaded()
        {
            mapBanner.sprite = BannerAsset;
        }

        public void OnPointerClick(PointerEventData pointerEventData) => onClickInstance?.Invoke(ObjectData);
    }
}
