using M7.Match;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Assets/M7/GameData/LevelData")]
    public class LevelData : ScriptableObject
    {
        [JsonIgnore] [ShowInInspector, PropertyOrder(-1)] public string MasterID => name;

        public enum GameModeType { Adventure, PVP }

        [JsonProperty] [SerializeField] GameModeType gameMode = GameModeType.Adventure;
        [JsonProperty] [SerializeField] TeamData_Enemy teamData;
        [JsonIgnore] [SerializeField] AssetReferenceT<GameObject> environment;
        [JsonProperty] [SerializeField] MatchGridEditorSavedLevelData matchGridEditorSavedLevelData;
        [JsonProperty] [SerializeField] int totalMoveCount = 15;
        [JsonProperty] [SerializeField] GoalData[] goalConditions;

        [JsonIgnore] public GameModeType GameMode => gameMode;
        [JsonIgnore] public string DisplayName => MasterID.Replace("_", " ");
        [JsonIgnore] public TeamData_Enemy TeamData => teamData;
        [JsonIgnore] public AssetReferenceT<GameObject> Environment => environment;
        [JsonIgnore] public MatchGridEditorSavedLevelData MatchGridEditorSavedLevelData => matchGridEditorSavedLevelData;
        [JsonIgnore] public int TotalMoveCount => int.Parse(PlayerDatabase.CampaignData.customStage.Replace("Stage_", "")) >= int.Parse(PlayerDatabase.CampaignData.currentStage.Replace("Stage_", "")) ? PlayerDatabase.GlobalDataSetting.StageAllDataSettings.GetStageDataSettings(PlayerDatabase.CampaignData.currentStage).totalMoveCount : PlayerDatabase.GlobalDataSetting.StageAllDataSettings.GetStageDataSettings(PlayerDatabase.CampaignData.customStage).totalMoveCount;
        [JsonIgnore] public float StageBonus => int.Parse(PlayerDatabase.CampaignData.customStage.Replace("Stage_", "")) >= int.Parse(PlayerDatabase.CampaignData.currentStage.Replace("Stage_", "")) ? PlayerDatabase.GlobalDataSetting.StageAllDataSettings.GetStageDataSettings(PlayerDatabase.CampaignData.currentStage).stageBonus : PlayerDatabase.GlobalDataSetting.StageAllDataSettings.GetStageDataSettings(PlayerDatabase.CampaignData.customStage).stageBonus;
        [JsonIgnore] public float GaianiteCap => int.Parse(PlayerDatabase.CampaignData.customStage.Replace("Stage_", "")) >= int.Parse(PlayerDatabase.CampaignData.currentStage.Replace("Stage_", "")) ? PlayerDatabase.GlobalDataSetting.StageAllDataSettings.GetStageDataSettings(PlayerDatabase.CampaignData.currentStage).gaianiteCap : PlayerDatabase.GlobalDataSetting.StageAllDataSettings.GetStageDataSettings(PlayerDatabase.CampaignData.customStage).gaianiteCap;
        [JsonIgnore] public GoalData[] GoalConditions => goalConditions;

        [JsonIgnore] public int StageValue => int.Parse(MasterID.Replace("Stage_", ""));
    }
}
