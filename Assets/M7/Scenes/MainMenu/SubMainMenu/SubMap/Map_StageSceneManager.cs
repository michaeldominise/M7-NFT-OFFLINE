using System;
using M7.CDN.Addressable;
using M7.GameData;
using M7.GameRuntime;
using System.Collections;
using System.Collections.Generic;
using M7.GameRuntime.Scripts.Energy;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;

namespace M7
{
    public class Map_StageSceneManager : SceneManagerBase
    {
        public static Map_StageSceneManager Instance { get; private set; }

        [SerializeField] PopulateInventoryManager<LevelInstance_Map, LevelData> populateInventoryManager;
        [SerializeField] RectTransform content;
        [SerializeField] float yCenterOffset = 200;
        [SerializeField] ChapterDataList chapterDataList;
        [SerializeField] AssetReference stageInfoScene;
        [SerializeField] ScrollRect scrollRect;
        public static ChapterData SelectedChapter { get; set; }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private IEnumerator Start()
        {
            populateInventoryManager.Populate(SelectedChapter.LevelDataList, OnItemClick);
            yield return new WaitForEndOfFrame();
            var index = Mathf.Clamp(chapterDataList.GetCurrentLevelData().StageValue - SelectedChapter.FirstLevelData.StageValue, 0, populateInventoryManager.ItemSlotList.Count - 1);
            DOTween.To(() => 0, newYValue => content.anchoredPosition = new Vector2(content.anchoredPosition.x, newYValue), yCenterOffset - populateInventoryManager.ItemSlotList[index].transform.localPosition.y, 1).onComplete += () => StartCoroutine(DelayOnComplete());
        }

        private void Update() => populateInventoryManager.Update();

        private void OnItemClick(LevelData data)
        {
            if (data.StageValue > chapterDataList.GetCurrentLevelData().StageValue)
                return;

            LevelManager.ChapterData = SelectedChapter;
            LevelManager.LevelData = data;
            LoadScene(stageInfoScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;

            }
        }

        private IEnumerator DelayOnComplete()
        {
            yield return new WaitForSeconds(1);
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
        }
    }
}