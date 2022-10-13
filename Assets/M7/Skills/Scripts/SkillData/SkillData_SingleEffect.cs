using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;

namespace M7.Skill
{
    [Serializable]
    public class SkillData_Singleffect : SkillData
    {
        [SerializeField] protected StatusEffect statusEffect;
        public override List<StatusEffect> StatusEffects => new List<StatusEffect> { statusEffect };
        [ShowInInspector] public override float Value
        {
            get => statusEffect?.Value ?? 0;
#if UNITY_EDITOR
            set { if (statusEffect) statusEffect.Value = value; }
#endif
        }
    }
}