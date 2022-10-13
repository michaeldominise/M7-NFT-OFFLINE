using M7.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace M7.GameData
{
    [System.Serializable]
    public class CombatStats
    {
        [JsonProperty] [SerializeField] public float hp = 10;
        [JsonProperty] [SerializeField] public float attack = 10;
        [JsonProperty] [SerializeField] public float defense = 0;
        [JsonProperty] [SerializeField] public float passion = 0;
        [JsonProperty] [SerializeField] public float luck = 0;
        [JsonProperty] [SerializeField] public float durability = 1;

        [JsonIgnore] public float Hp => hp;
        [JsonIgnore] public float Attack => attack;
        [JsonIgnore] public float Defense => defense;
        [JsonIgnore] public float Passion => passion;
        [JsonIgnore] public float Luck => luck;
        [JsonIgnore] public float Durability => durability;

        internal float GetStatsValue(SkillEnums.TargetCharacterStats targetStats)
        {
            switch (targetStats)
            {
                case SkillEnums.TargetCharacterStats.CurrentHp:
                case SkillEnums.TargetCharacterStats.MaxHp:
                    return Hp;
                case SkillEnums.TargetCharacterStats.Attack:
                    return Attack;
                case SkillEnums.TargetCharacterStats.Defense:
                    return Defense;
                case SkillEnums.TargetCharacterStats.Passion:
                    return Passion;
                case SkillEnums.TargetCharacterStats.Luck:
                    return Luck;
                case SkillEnums.TargetCharacterStats.Durability:
                    return Durability;
            }
            return 0;
        }

        public void AddValues(ref float hp, ref float attack, ref float defense, ref float mining, ref float luck, ref float durability)
        {
            hp += this.hp;
            attack += this.attack;
            defense += this.defense;
            mining += this.passion;
            luck += this.luck;
            durability += this.durability;
        }

        public void MinusValues(float durability)
        {
            this.durability -= durability;
        }
    }
}