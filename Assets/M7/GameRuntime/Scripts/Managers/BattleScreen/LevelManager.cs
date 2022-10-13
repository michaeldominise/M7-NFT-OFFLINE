using M7.CDN.Addressable;
using M7.GameData;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance => BattleManager.Instance?.LevelManager;

        static ChapterData chapterData;
        public static ChapterData ChapterData { get => chapterData ?? Instance?.sampleChapterData; set => chapterData = value; }

        static LevelData levelData;
        public static LevelData LevelData
        {
            get
            {
#if UNITY_EDITOR
                var levelPreview = Tools.LevelPreview.Instance;
                if (levelPreview != null && levelPreview.isInPreview)
                {
                    return Tools.LevelPreview.Instance.previewLevelData;
                }
#endif
                return levelData ?? Instance?.sampleLevelData;
            }
            set => levelData = value;
        }
        public static LevelData.GameModeType GameMode => LevelData?.GameMode ?? LevelData.GameModeType.Adventure;

        [SerializeField] ChapterData sampleChapterData;
        [SerializeField] LevelData sampleLevelData;


        internal void Init(Action onFinish)
        {
            if (LevelData.Environment == null)
            {
                onFinish?.Invoke();
                return;
            }

            LevelData.Environment.LoadAssetAsync(environment =>
            {
                var environmentObject = Instantiate(environment.GetComponent<GameData.Environment>(), Camera.main.transform);
                environmentObject.Init(Camera.main);
                onFinish?.Invoke();
            });

        }
    }
}