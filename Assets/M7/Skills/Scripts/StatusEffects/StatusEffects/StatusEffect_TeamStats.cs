using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_TeamStats : StatusEffect_InputValue<SkillEnums.TargetTeamStats>
    {
        public override void ApplyPermanentEffect(StatusEffectInstance statusEffectInstance, bool isHeal, Action onFinish)
        {
            var teamTarget = statusEffectInstance.Target as TeamManager_Battle;
            teamTarget.StatsInstance.AddValue((statusEffectInstance.StatusEffect as StatusEffect_TeamStats).TargetStats, GetInstanceInputValue(statusEffectInstance, teamTarget.StatsInstance.GetValue(TargetStats, false)));
            onFinish?.Invoke();
        }
    }
}