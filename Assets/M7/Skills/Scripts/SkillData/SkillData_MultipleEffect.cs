using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;

namespace M7.Skill
{
    [Serializable]
    public class SkillData_MultipleEffect : SkillData
    {
        [SerializeField] protected List<StatusEffect> statusEffects;
        public override List<StatusEffect> StatusEffects => statusEffects;

        [ShowInInspector] public override float Value
        {
            get => statusEffects?.Sum(x => x?.Value ?? 0) ?? 0;
#if UNITY_EDITOR
            set {  }
#endif
        }

#if UNITY_EDITOR
        public string effectId;
#endif
    }
}