using Newtonsoft.Json;
using UnityEngine;
using M7.Skill;
using System;
using Sirenix.OdinInspector;

namespace M7.GameData
{
    [System.Serializable]
    public class SaveableCharacterData : BaseSaveableData<CharacterObject>
    {
        [JsonProperty, SerializeField] public string displayName;
        [JsonProperty, SerializeField] public int mintCount;
        [JsonProperty, FoldoutGroup("Level"), SerializeField] public int level;
        [JsonProperty, FoldoutGroup("Level"), ShowInInspector, DisplayAsString(false)] public DateTime levelUpTimeComplete;
        [JsonProperty, FoldoutGroup("Stats"), SerializeField] public int availableStatsPoints;
        [JsonProperty, FoldoutGroup("Stats"), SerializeField] public CombatStats baseStats = new CombatStats();
        [JsonProperty, FoldoutGroup("Stats"), SerializeField] public CombatStats saveableStats = new CombatStats();
        [JsonProperty, SerializeField] public AncestorData ancestorData = new AncestorData();
        [JsonProperty, SerializeField] public SocketsData [] sockets;
        [JsonProperty, SerializeField] public bool isNonNFT;

        [JsonProperty, SerializeField] public string[] equipmentIDs;

# if UNITY_EDITOR
        [JsonIgnore] CharacterObject CharacterObject => AssetReferenceData.Asset as CharacterObject;
        [JsonIgnore] public SkillEnums.RarityFilter rarity => CharacterObject?.DisplayStats.Rarity ?? SkillEnums.RarityFilter.Common;
        [JsonIgnore] public SkillEnums.ElementFilter elementType => CharacterObject?.Element.ElementType ?? SkillEnums.ElementFilter.Fire;
        [JsonIgnore] public string skillId => (CharacterObject?.CharacterSkillObject.editorAsset.GetComponent<SkillObject>())?.MasterID;
        [JsonIgnore] public int skillCost
        {
            get
            {
                if (CharacterObject == null || CharacterObject.CharacterSkillConditionList.Length == 0)
                    return 0;
                return (int)CharacterObject.CharacterSkillConditionList[0].RequiredPoints;
            }
        }
# endif
        [JsonIgnore] public string DisplayName => displayName;
        [JsonIgnore] public int Level => level;
        [JsonIgnore] public DateTime LevelUpTimeComplete => levelUpTimeComplete;
        [JsonIgnore] public int MintCount => mintCount;
        [JsonIgnore] public int AvailableStatsPoints => availableStatsPoints;
	    [JsonIgnore] public int MintMax => 7;
	    [JsonIgnore] public CombatStats BaseStats => baseStats;
	    [JsonIgnore] public CombatStats SaveableStats => saveableStats;
        [JsonIgnore] public AncestorData AncestorDatas => ancestorData;
        [JsonIgnore] public SocketsData [] Sockets => sockets;
        [JsonIgnore] public bool IsNonNFT => isNonNFT;
        [JsonIgnore] public string[] EquipmentIDs { get => equipmentIDs; private set => equipmentIDs = value; }


        public SaveableCharacterData() { }

        public SaveableCharacterData(string masterID, string instanceID, int level, string[] equipmentIDs) : base(masterID, instanceID)
        {
            this.level = level;
            this.equipmentIDs = equipmentIDs;
        }

        //public string GetAdjectiveName(string realName) => displayName.Replace($" {realName}", "");
        public string GetAdjectiveName(string realName) => realName;
    }
}