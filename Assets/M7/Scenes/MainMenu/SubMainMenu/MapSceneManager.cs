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
    public class MapSceneManager : SceneManagerBase
    {
        public static MapSceneManager Instance { get; private set; }

        [SerializeField] PopulateInventoryManager<ChapterInstance_Map, ChapterData> populateInventoryManager;
        [SerializeField] RectTransform content;
        [SerializeField] float yCenterOffset = 200;
        [SerializeField] AssetReference stageSelect;
        [SerializeField] ChapterDataList chapterDataList;
        [SerializeField] ScrollRect scrollRect;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private IEnumerator Start()
        {
            populateInventoryManager.Populate(chapterDataList.AllChapters.ToList(), OnItemClick);
            yield return new WaitForEndOfFrame();
            DOTween.To(() => 0, newYValue => content.anchoredPosition = new Vector2(content.anchoredPosition.x, newYValue), yCenterOffset - populateInventoryManager.ItemSlotList[chapterDataList.GetCurrentChapterData().Index].transform.localPosition.y, 1).onComplete += () => StartCoroutine(DelayOnComplete());
        }
        private void Update() => populateInventoryManager.Update();

        private void OnItemClick(ChapterData data)
        {
            LoadScene(stageSelect, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            Map_StageSceneManager.SelectedChapter = data;
        }

        private IEnumerator DelayOnComplete()
        {
            yield return new WaitForSeconds(1);
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
        }
    }
}