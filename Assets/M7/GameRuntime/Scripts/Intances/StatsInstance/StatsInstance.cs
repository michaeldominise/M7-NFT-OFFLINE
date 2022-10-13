using M7.GameData;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public abstract class StatsInstance<TargetInstanceType, InstanceActionsType, EnumType> where InstanceActionsType : InstanceActions, new() where EnumType : Enum
    {
        public InstanceActionsType InstanceActions { get; protected set; } = new InstanceActionsType();
        protected TargetInstanceType TargetInstance { get; set; }
        protected abstract List<StatusEffectInstance> StatusEffectInstanceLedger { get; }

        public void Init(TargetInstanceType targetInstance) => TargetInstance = targetInstance;
        public virtual void OnStatusEffectInstanceLedgerUpdate(StatusEffectInstance updatedStatusEffectInstance, IStatusEffectInstanceController.UpdateType updateType) { }
        public abstract float GetValue(EnumType statType, bool addLedgerValues = true);
        protected virtual float GetValueWithLedger(float baseValue, EnumType statType)
        {
            var value = baseValue;
            if (StatusEffectInstanceLedger != null)
                foreach (var statusEffectInstance in StatusEffectInstanceLedger)
                {
                    var statusEffect_InputValue = statusEffectInstance.StatusEffect as StatusEffect_InputValue<EnumType>;
                    if (statusEffect_InputValue == null)
                        continue;

                    if (statType.Equals(statusEffect_InputValue.TargetStats))
                        value += statusEffect_InputValue.GetInstanceInputValue(statusEffectInstance, baseValue);
                }
            return value;
        }

        public virtual void SetValue(EnumType statType, float value) { }
        public virtual void AddValue(EnumType statType, float value) => SetValue(statType, GetValue(statType, false) + value);
        public virtual void InitStatValues() => UpdateInstanceActions();
        public abstract void UpdateInstanceActions();
    }
}
