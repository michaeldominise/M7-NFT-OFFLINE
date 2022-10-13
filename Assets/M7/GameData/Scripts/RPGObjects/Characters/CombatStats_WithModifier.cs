using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameData
{
    [System.Serializable]
    public class CombatStats_WithModifier : CombatStats
    {
        [SerializeField] float hpModifier = 10;
        [SerializeField] float attackModifier = 10;
        [SerializeField] float defenseModifier = 1;
        [SerializeField] float miningModifier = 0.01f;
        [SerializeField] float luckModifier = 0.01f;
        [SerializeField] float durabilityModifier = 0.01f;

        public float HpModifier => hpModifier;
        public float AttackModifier => attackModifier;
        public float DefenseModifier => defenseModifier;
        public float MiningModifier => miningModifier;
        public float LuckModifier => luckModifier;
        public float DurabilityModifier => durabilityModifier;
    }
}