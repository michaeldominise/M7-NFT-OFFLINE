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
using M7.ServerTestScripts;

namespace M7.GameRuntime
{
    public class LevelInstance_Map : BaseLevelInstance, IPointerClickHandler
    {
        public static LevelInstance_Map Instance;
        [SerializeField] Sprite[] stateSprites;
        [SerializeField] Image stageImage;
        [SerializeField] TextMeshProUGUI stageLabel;
        [SerializeField] GameObject pointer;
        void Awake()
        {
            Instance = this;
        }
        public override void Init(LevelData objectData, Action onFinish)
        {
            if (this.ObjectData?.MasterID == objectData.MasterID)
                return;

            ObjectData = objectData;
            var currentStage = int.Parse(PlayerDatabase.CampaignData.currentStage.Replace("Stage_", ""));

            var objectStage = ObjectData.StageValue;
            stageLabel.text = $"{objectStage:00}";
            stageLabel.transform.parent.gameObject.SetActive(objectStage < currentStage);
            pointer.SetActive(objectStage == currentStage);
            if (objectStage == currentStage)
                stageImage.sprite = stateSprites[0];
            else if (objectStage < currentStage)
                stageImage.sprite = stateSprites[1];
            else
                stageImage.sprite = stateSprites[2];

            onFinish?.Invoke();
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            PlayerDatabase.CampaignData.customStage = "Stage_" + ObjectData.StageValue;
            //DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.UpdateCustomStage, PlayerDatabase.CampaignData.customStage);
            onClickInstance?.Invoke(LevelData);
        }
    }
}
