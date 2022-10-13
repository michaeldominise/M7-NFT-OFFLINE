using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.PlayfabCloudscript;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "ChapterDataList", menuName = "Assets/M7/GameData/ChapterDataList")]
    public class ChapterDataList : ScriptableObject
    {
        [SerializeField] ChapterData[] allChapters;
        public ChapterData[] AllChapters => allChapters;

        public LevelData GetCurrentLevelData() => GetLevelData(PlayerDatabase.CampaignData.currentStage);
        public ChapterData GetCurrentChapterData() => GetChapterData(PlayerDatabase.CampaignData.currentStage);
        public LevelData GetLevelData(string stageID)
        {
            for (int i = 0; i < AllChapters.Length; i++)
                for (int x = 0; x < AllChapters[i].LevelDataList.Count; x++)
                    if (AllChapters[i].LevelDataList[x].MasterID == stageID)
                        return AllChapters[i].LevelDataList[x];

            return null;
        }
        public ChapterData GetChapterData(string stageID)
        {
            for (int i = 0; i < AllChapters.Length; i++)
                for (int x = 0; x < AllChapters[i].LevelDataList.Count; x++)
                    if (AllChapters[i].LevelDataList[x].MasterID == stageID)
                        return AllChapters[i];

            return null;
        }
    }
}
