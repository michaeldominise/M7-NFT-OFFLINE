using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace M7.GameData
{
    [Serializable]
    public class GlobalDataSetting
    {
        [SerializeField] StageAllDataSettings stageAllData = new StageAllDataSettings();
        [SerializeField] PlayerAllLevelChartData playerAllLevelChartData = new PlayerAllLevelChartData();
        [SerializeField] RecoveryCostAllChartData recoveryCostAllChartData = new RecoveryCostAllChartData();
        public StageAllDataSettings StageAllDataSettings => stageAllData;
        public PlayerAllLevelChartData PlayerAllLevelChartData => playerAllLevelChartData;
        public RecoveryCostAllChartData RecoveryCostAllChartData => recoveryCostAllChartData;
    }
    [Serializable]
    public class StageAllDataSettings
    {
        [JsonProperty] [SerializeField] List <StageDataSettings> stageDataSettingsList;
        [JsonIgnore] public List<StageDataSettings> StageDataSettings => stageDataSettingsList;

        public void OverWrite_Stage(string json)
        {
            stageDataSettingsList = JsonConvert.DeserializeObject<List<StageDataSettings>>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
        }
        public StageDataSettings GetStageDataSettings(string stageId) => StageDataSettings.FirstOrDefault(x => x.stageId == stageId) ?? new GameData.StageDataSettings();
    }

    [Serializable]
    public class PlayerAllLevelChartData
    {
        [JsonProperty] [SerializeField] List<PlayerLevelChartData> playerLevelChartDataSettings;
        [JsonIgnore] public List<PlayerLevelChartData> PlayerLevelChartDataSettings => playerLevelChartDataSettings;

        public void OverWrite_Stage(string json)
        {
            playerLevelChartDataSettings = JsonConvert.DeserializeObject<List<PlayerLevelChartData>>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
        }
        public PlayerLevelChartData GetPlayerChart(int _Level) => PlayerLevelChartDataSettings.FirstOrDefault(x => x.levels == _Level);
    }


    [Serializable]
    public class RecoveryCostAllChartData
    {
        [JsonProperty] [SerializeField] RarityList recoveryCostDataSettings;
        [JsonIgnore] public RarityList RecoveryCost => recoveryCostDataSettings;

        public void OverWrite_Recovery(string json)
        {
            recoveryCostDataSettings = JsonConvert.DeserializeObject<RarityList>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
        }
        //public RarityList GetRecoveryCost(string _rarity, int _Level)
        //{
        //     return rarityListDataSettings.
        //}
    }

    [Serializable]
    public class StageDataSettings
    {
        public string stageId = "PreviewStage";
        public float stageBonus = 0.01f;
        public float gaianiteCap = 10;
        public int totalMoveCount = 10;
    }
    [Serializable]
    public class PlayerLevelChartData
    {
        public int levels;
        public float time;
        public float gaiCost;
        public float m7Cost;
    }
    [Serializable]
    public class RarityList
    {
        public float[] common_cost;
        public float[] uncommon_cost;
        public float[] rare_cost;
        public float[] epic_cost;
        public float[] legendary_cost;
    }
}