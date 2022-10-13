using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace M7.GameData
{
    [Serializable]
    public class CampaignData
    {
        [JsonProperty] [SerializeField] public string currentStage = "Stage_1";
        [JsonProperty] [SerializeField] public string customStage = "Stage_1";
        [JsonProperty] [SerializeField] List<SaveableStageData> saveableStageDataList;

        [JsonIgnore] public List<SaveableStageData> SaveableStageDatas => saveableStageDataList;

        public void OverwriteValues(string json)
        {
            CampaignData campaignData = JsonConvert.DeserializeObject<CampaignData>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
            currentStage = campaignData.currentStage;
            customStage = campaignData.customStage;
            saveableStageDataList = campaignData.saveableStageDataList;
        }

        //public void OverwriteValues_StageBonus(string json)
        //{
        //    List<CampaignData> campaignData = JsonConvert.DeserializeObject<List<CampaignData>>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
            
        //    foreach(CampaignData campData in campaignData)
        //        stageBonus = campData.stageBonus;
        //}
        //public void OverwriteValues_GaianiteCap(string json)
        //{
        //    List<CampaignData> campaignData = JsonConvert.DeserializeObject<List<CampaignData>>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });

        //    foreach (CampaignData campData in campaignData)
        //        gaianiteCap = campData.gaianiteCap;
        //}
    }

    [Serializable]
    public class SaveableStageData
    {
        [JsonProperty] [SerializeField] string stageID;
        [JsonProperty] [SerializeField] public int starsCompleted;

        [JsonIgnore] public string StageID => stageID;
    }
}



