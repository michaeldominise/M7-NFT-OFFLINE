using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using M7.GameData;
using UnityEngine;
using static M7.Skill.SkillEnums;

namespace M7.QATester
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<HeroIngameJsonData>(myJsonResponse);

    [Serializable]
    public class HeroIngameJsonData
    {
        public int level { get; set; }
        public int mintCount { get; set; }
        public BaseStats baseStats { get; set; }
        public SaveableStats saveableStats { get; set; }
        [JsonProperty, SerializeField] public RarityFilter rarity  { get; set; }
        [JsonProperty, SerializeField] public ElementFilter element { get; set; }
        public string skillId { get; set; }
        public int skillCost { get; set; }
        public AncestorData ancestorData { get; set; }
        public string[] equipmentIDs { get; set; }
        [JsonProperty, SerializeField] public SocketsData[] sockets { get; set; }
        public string masterID { get; set; }
        public string displayName { get; set; }
        public string instanceID { get; set; }
        public string levelUpTimeComplete { get; set; }
        public int availableStatsPoints { get; set; }
    }

    [Serializable]
    public class AncestorData
    {
        public List<object> parentIDs { get; set; }
        public List<object> grandParentIDs { get; set; }
    }

    [Serializable]
    public class BaseStats
    {
        public int attack { get; set; }
        public int luck { get; set; }
        public int passion { get; set; }
        public int hp { get; set; }
        public int durability { get; set; }
    }

    [Serializable]
    public class SaveableStats
    {
        public int attack { get; set; }
        public int luck { get; set; }
        public int passion { get; set; }
        public int hp { get; set; }
        public int durability { get; set; }
    }


}