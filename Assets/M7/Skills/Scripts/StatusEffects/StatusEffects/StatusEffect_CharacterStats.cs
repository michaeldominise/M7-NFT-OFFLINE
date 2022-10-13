using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_CharacterStats : StatusEffect_InputValue<SkillEnums.TargetCharacterStats>
    {
        [ShowIf("@computationType == SkillEnums.ComputationType.Decrease && TargetStats == SkillEnums.TargetCharacterStats.CurrentHp")]
        [ShowInInspector] public bool IgnoreDefense;

        public override void ApplyPermanentEffect(StatusEffectInstance statusEffectInstance, bool isHeal, Action onFinish)
        {
            var charCaster = statusEffectInstance.Caster as CharacterInstance_Battle;
            var charTarget = statusEffectInstance.Target as CharacterInstance_Battle;

            if (TargetStats == SkillEnums.TargetCharacterStats.CurrentHp)
            {
                if (!isHeal)
                {
                    if (charCaster != null)
                    {
                        var tileTypeMatchData = MatchInterpreter.Instance.GetTileTypeMatchData(charCaster.Element.ElementType);
                        (charTarget.StatsInstance as StatsInstance_CharacterBattle).DamageThis(charCaster, statusEffectInstance.StatusEffectId, GetInstanceInputValue(statusEffectInstance, charTarget.StatsInstance.GetValue(TargetStats, false)) * tileTypeMatchData.GetDamageMultiplier(charTarget.Element.ElementType), tileTypeMatchData.GetDamageType(charTarget.Element.ElementType));
                    }
                    else
                        (charTarget.StatsInstance as StatsInstance_CharacterBattle).DamageThis(charCaster, statusEffectInstance.StatusEffectId, GetInstanceInputValue(statusEffectInstance, charTarget.StatsInstance.GetValue(TargetStats, false)), UIStatusValueManager.DamageType.Normal);
                }
                else
                {
                    if (charCaster != null)
                    {
                        var tileTypeMatchData = MatchInterpreter.Instance.GetTileTypeMatchData(charCaster.Element.ElementType);
                        (charTarget.StatsInstance as StatsInstance_CharacterBattle).HealThis(charCaster, statusEffectInstance.StatusEffectId, GetInstanceInputValue(statusEffectInstance, charTarget.StatsInstance.GetValue(TargetStats, false)) * tileTypeMatchData.GetDamageMultiplier(charTarget.Element.ElementType), tileTypeMatchData.GetDamageType(charTarget.Element.ElementType));
                    }
                    else
                        (charTarget.StatsInstance as StatsInstance_CharacterBattle).HealThis(charCaster, statusEffectInstance.StatusEffectId, GetInstanceInputValue(statusEffectInstance, charTarget.StatsInstance.GetValue(TargetStats, false)), UIStatusValueManager.DamageType.Normal);
                }
            }
            else
                charTarget.StatsInstance.AddValue(TargetStats, GetInstanceInputValue(statusEffectInstance, charTarget.StatsInstance.GetValue(TargetStats, false)));

            onFinish?.Invoke();
        }
    }
}