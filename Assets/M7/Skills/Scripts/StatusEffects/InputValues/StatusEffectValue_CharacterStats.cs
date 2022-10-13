using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace M7.Skill
{
    public class StatusEffectValue_CharacterStats : StatusEffectValue
    {
        public enum InputReference { Caster, Target }
        [SerializeField] InputReference inputReference;
        [SerializeField] SkillEnums.TargetCharacterStats targetStats;
        [SerializeField] SkillEnums.ComputationType modifierComputation;
        [SerializeField] float modifier;

        [ShowInInspector, DisplayAsString(false)] public override string DebugText => $"Get {inputReference}'s {targetStats} {modifierComputation} by {modifier}";

        public override float GetValue(StatusEffectInstance statusEffectInstance)
        {
            var charReference = (inputReference == InputReference.Caster ? statusEffectInstance?.Caster : statusEffectInstance?.Target) as CharacterInstance_Battle;
            if (!charReference)
                return 0;
            return modifierComputation.Calculate(charReference.StatsInstance.GetValue(targetStats), modifier);
        }

        public override float InputValue
        {
            get => modifier;
#if UNITY_EDITOR
            set 
            { 
                base.InputValue = value; 
                modifier = value; 
            }
#endif
        }
    }
}