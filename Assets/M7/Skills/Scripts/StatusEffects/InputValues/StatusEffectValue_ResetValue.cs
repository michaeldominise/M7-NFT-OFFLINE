using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace M7.Skill
{
    public abstract class StatusEffectValue_ResetValue<TargetStatsType> : StatusEffectValue where TargetStatsType : System.Enum
    {
        [SerializeField, HideInInspector] protected float inputValue;
        [SerializeField] protected TargetStatsType targetStats;

        public override string DebugText => $"Get a value that will reset {targetStats} by {InputValue}.";

        [ShowInInspector] public override float InputValue
        {
            get => inputValue;
#if UNITY_EDITOR
            set
            {
                base.InputValue = value;
                inputValue = value;
            }
#endif
        }

        public override float GetValue(StatusEffectInstance statusEffectInstance) => InputValue;
    }
}