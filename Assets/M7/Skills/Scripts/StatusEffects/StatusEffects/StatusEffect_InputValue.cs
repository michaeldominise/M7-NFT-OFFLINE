using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public abstract class StatusEffect_InputValue<EnumType> : StatusEffect where EnumType : Enum
    {
        [SerializeField] protected bool applyAsPermanent = true;
        [SerializeField] protected SkillEnums.ComputationType computationType;
        [SerializeField, ShowIf("ModifierComputationIsNotNone")] protected StatusEffectValue inputValue;
        [SerializeField] EnumType targetStats;

        protected bool ModifierComputationIsNotNone => computationType != SkillEnums.ComputationType.Multiply;
        [ShowInInspector, DisplayAsString(false)]
        public override string DebugText
            => base.DebugText
            + $"{computationType} Target's {targetStats} by {(ModifierComputationIsNotNone ? inputValue?.DebugText.Substring(4) : "")}";

        public bool ApplyAsPermanent => applyAsPermanent;
        public virtual EnumType TargetStats => targetStats;
        public SkillEnums.ComputationType ComputationType => computationType;

        public override float Value
        {
            get => inputValue?.InputValue ?? 0;
#if UNITY_EDITOR
            set => inputValue.InputValue = value;
#endif
        }

        public virtual float GetInputValue(StatusEffectInstance statusEffectInstance) => inputValue?.GetValue(statusEffectInstance) ?? 0;
        public virtual float GetInstanceInputValue(StatusEffectInstance statusEffectInstance, float baseValue) => Calculate(baseValue, (statusEffectInstance.StatusEffectData as StatusEffectData_InputValue).ModifierValue) - baseValue;
        public virtual float Calculate(float baseValue, float modifier) => computationType.Calculate(baseValue, modifier);
        public override StatusEffectData GenerateStatusEffectData(StatusEffectInstance statusEffectInstance) => new StatusEffectData_InputValue { ModifierValue = GetInputValue(statusEffectInstance) };
        public override void Execute(StatusEffectInstance statusEffectInstance, Action onFinish)
        {
            if (applyAsPermanent)
            {
                if (computationType == SkillEnums.ComputationType.Increase)
                    ApplyPermanentEffect(statusEffectInstance, true, onFinish);
                else
                    ApplyPermanentEffect(statusEffectInstance, false, onFinish);
            }
            else
                base.Execute(statusEffectInstance, onFinish);
        }
        public abstract void ApplyPermanentEffect(StatusEffectInstance statusEffectInstance, bool isHeal, Action onFinish);
    }
}