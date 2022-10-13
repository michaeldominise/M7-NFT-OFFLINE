using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using M7.Skill;

namespace M7.GameData
{
    [System.Serializable]
    public class SocketsData
    {
        public string gemId;
        [JsonIgnore] public SkillEnums.TargetCharacterStats statsTypeValue;
        [JsonIgnore] public SkillEnums.RarityFilter rarityValue;
        public string statsType {
            get => statsTypeValue switch
            {
                SkillEnums.TargetCharacterStats.MaxHp => "Hp",
                SkillEnums.TargetCharacterStats.Attack => "Attack",
                SkillEnums.TargetCharacterStats.Luck => "Luck",
                SkillEnums.TargetCharacterStats.Passion => "Passion",
                _ => "",
            };
            set
            {
                statsTypeValue = value switch
                {
                    "Hp" => SkillEnums.TargetCharacterStats.MaxHp,
                    "Attack" => SkillEnums.TargetCharacterStats.Attack,
                    "Luck" => SkillEnums.TargetCharacterStats.Luck,
                    "Passion" => SkillEnums.TargetCharacterStats.Passion,
                    _ => SkillEnums.TargetCharacterStats.MaxHp
                };
            }
        }

        public string rarity {
            get => rarityValue.ToString();
            set
            {
                rarityValue = value switch
                {
                    "Common" => SkillEnums.RarityFilter.Common,
                    "Uncommon" => SkillEnums.RarityFilter.Uncommon,
                    "Rare" => SkillEnums.RarityFilter.Rare,
                    "Epic" => SkillEnums.RarityFilter.Epic,
                    "Legendary" => SkillEnums.RarityFilter.Legendary,
                    _ => SkillEnums.RarityFilter.All
                };
            }
        }
    }
}
